using AngularEshop.Core.DTOs.Paging;
using AngularEshop.Core.DTOs.Products;
using AngularEshop.Core.Services.Interfaces;
using AngularEshop.Core.Utilities.Common;
using AngularEshop.Core.Utilities.Extensions.FileExtensions;
using AngularEshop.Core.Utilities.Extensions.Paging;
using AngularEshop.DataLayer.Entities.Account;
using AngularEshop.DataLayer.Entities.Product;
using AngularEshop.DataLayer.Ripository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularEshop.Core.Services.Implementations
{
    public class ProductService : IProductService
    {
        
        #region constructor
        private IGenericRipository<Product> productRipository;
        private IGenericRipository<ProductCategory> productCategoryRipository;
        private IGenericRipository<ProductSelectedCategory> productSelectedCategoryRipository;
        private IGenericRipository<ProductGallery> productGalleryRipository;
        private IGenericRipository<ProductVisit> productVisitRipository;
        private IGenericRipository<ProductComment> productCommentRipository;
        public ProductService(IGenericRipository<Product> productRipository,
            IGenericRipository<ProductCategory> productCategoryRipository,
            IGenericRipository<ProductSelectedCategory> productSelectedCategoryRipository,
             IGenericRipository<ProductGallery> productGalleryRipository,
             IGenericRipository<ProductVisit> productVisitRipository,
             IGenericRipository<ProductComment> productCommentRipository
             )
            
        {
            this.productRipository = productRipository;
            this.productCategoryRipository = productCategoryRipository;
            this.productSelectedCategoryRipository = productSelectedCategoryRipository;
            this.productGalleryRipository = productGalleryRipository;
            this.productVisitRipository = productVisitRipository;
            this.productCommentRipository = productCommentRipository;

        }


        #endregion
        #region product
       public async Task AddProduct(Product product)
        {
            await productRipository.AddEntity(product);
            await productRipository.SaveChanges();
        }

       public async Task UpdateProduct(Product product)
        {
            productRipository.UpdateEntity(product);
            await productRipository.SaveChanges();
        }
        public async Task<FilterProductsDTO> FilterProducts(FilterProductsDTO filter)
        {
            var productsQuery = productRipository.GetEntitiesQuery().AsQueryable();
            switch (filter.OrderBy)
            {

                case ProductOrderBy.PriceAsc:
                    productsQuery = productsQuery.OrderBy(s => s.Price);
                    break;
                case ProductOrderBy.PriceDec:
                    productsQuery = productsQuery.OrderByDescending(s => s.Price);
                    break;
                default:
                    break;
            }



            if (!string.IsNullOrEmpty(filter.Title))
            {
                productsQuery = productsQuery.Where(s => s.ProductName.Contains(filter.Title));
            }
            if (filter.StartPrice != null)
            {
                productsQuery = productsQuery.Where(s => s.Price >= filter.StartPrice);
            }
        
            if (filter.EndPrice != null)
            {
                productsQuery = productsQuery.Where(s => s.Price <= filter.EndPrice);
            }
            if (filter.Categories !=null && filter.Categories.Any())
            {
                productsQuery = productsQuery.SelectMany(s => s.ProductSelectedCategories.Where(f => filter.Categories.Contains(f.ProductCategoryId)).Select(t => t.Product));
            }
            var count = (int)Math.Ceiling(productsQuery.Count()/(double)filter.TakeEntity);
            var pager = Pager.Build(count, filter.PageId, filter.TakeEntity);
            var products = await productsQuery.Paging(pager).ToListAsync();
            return filter.SetProducts(products).SetPaging(pager);
        }
        public async Task<List<Product>> GetRelatedProducts(long productId)
        {
            var product = await productRipository.GetEntityById(productId);

            if (product == null) return null;
            var productCategoriesList = await productSelectedCategoryRipository.GetEntitiesQuery()
              .Where(s => s.ProductId == productId).Select(f => f.ProductCategoryId).ToListAsync();
            var relatedProducts = await productRipository
                 .GetEntitiesQuery()
                 .SelectMany(s =>
                     s.ProductSelectedCategories.Where(f => productCategoriesList.Contains(f.ProductCategoryId))
                         .Select(t => t.Product))
                 .Where(s => s.Id != productId)
                 .OrderByDescending(s => s.CreateDate)
                 .Take(4).ToListAsync();

            return relatedProducts;
        }
        public async Task<bool> IsExistsProductById(long productId)
        {
            return await productRipository.GetEntitiesQuery().AnyAsync(s => s.Id == productId);
        }
        public async Task<Product> GetProductForUserOrder(long productId)
        {
            return await productRipository.GetEntitiesQuery()
                 .SingleOrDefaultAsync(s => s.Id == productId && !s.IsDelete);
        }

        #region admin edit
        public async Task<EditProductDTO> GetProductForEdit(long productId)
        {
            var product = await productRipository.GetEntitiesQuery().AsQueryable()
                .SingleOrDefaultAsync(s => s.Id == productId);
            if (product == null) return null;

            return new EditProductDTO
            {
                Id = product.Id,
                CurrentImage = product.ImageName,
                Description = product.Description,
                IsExists = product.IsExists,
                IsSpecial = product.IsSpecial,
                Price = product.Price,
                ProductName = product.ProductName,
                ShortDescription = product.ShortDescription
            };
        }
        public async Task EditProduct(EditProductDTO product)
        {
            var mainProduct = await productRipository.GetEntityById(product.Id);
            if (mainProduct != null)
            {
                mainProduct.ProductName = product.ProductName;
                mainProduct.Description = product.Description;
                mainProduct.IsExists = product.IsExists;
                mainProduct.IsSpecial = product.IsSpecial;
                mainProduct.Price = product.Price;
                mainProduct.Description = product.Description;
                mainProduct.ShortDescription = product.ShortDescription;

                if (!string.IsNullOrEmpty(product.Base64Image))
                {
                    var imagefile = ImageUploaderExtension.Base64ToImage(product.Base64Image);
                    var imagename = Guid.NewGuid().ToString("n") + ".jpeg";
                    imagefile.AddImageToServer(imagename, PathTools.ProductImageServerPath, mainProduct.ImageName);
                    mainProduct.ImageName = imagename;
                }

                productRipository.UpdateEntity(mainProduct);
                await productRipository.SaveChanges();
            }
        }

        #endregion

        #endregion
        #region product categories
        public async Task<List<ProductCategory>> GetAllActiveProductCategories()
        {
            return await productCategoryRipository.GetEntitiesQuery().Where(s=>!s.IsDelete).ToListAsync();
        }

        public async Task<Product> GetProductById(long productId)
        {
     
            return await productRipository.GetEntitiesQuery().AsQueryable()
                .SingleOrDefaultAsync(s=>!s.IsDelete && s.Id==productId);
        }

        #endregion
        #region product gallery
        public async Task<List<SingelProductDTO>> GetProductActiveGalleries(long productId)
        {
            return await productGalleryRipository.GetEntitiesQuery()
                .Where(s => s.ProductId == productId&&!s.IsDelete)
                .Select(s=> new SingelProductDTO
                {
                    ProductId=s.ProductId,
                    Id=s.Id,
                    ImageName=s.ImageName,
                    CreateDate=s.CreateDate
                })
                .ToListAsync();
        }
        #endregion
        #region product comments
        public async Task<List<ProductCommentDTO>> GetActiveProductComments(long productId)
        {
            return await productCommentRipository
                .GetEntitiesQuery()
                .Include(s => s.User)
                .Where(c => c.ProductId == productId && !c.IsDelete)
                .OrderByDescending(s => s.CreateDate)
                .Select(s => new ProductCommentDTO
                {
                    Id = s.Id,
                    Text = s.Text,
                    UserId = s.UserId,
                    UserFullName = s.User.FirstName + " " + s.User.LastName,
                    CreateDate = s.CreateDate.ToString("yyyy/MM/dd HH:mm")
                }).ToListAsync();
        }

        public async Task<ProductCommentDTO> AddProductComment(AddProductCommentDTO comment, long userId)
        {
            var commentData = new ProductComment
            {
                ProductId = comment.ProductId,
                Text = comment.Text,
                UserId = userId
            };

            await productCommentRipository.AddEntity(commentData);

            await productCommentRipository.SaveChanges();

            return new ProductCommentDTO
            {
                Id = commentData.Id,
                CreateDate = commentData.CreateDate.ToString("yyyy/MM/dd HH:mm"),
                Text = commentData.Text,
                UserId = userId,
                UserFullName = ""
            };
        }

        #endregion
        #region Dispose
        public void Dispose()
        {
            productRipository?.Dispose();
            productCategoryRipository?.Dispose();
            productSelectedCategoryRipository?.Dispose();
            productGalleryRipository?.Dispose();
            productVisitRipository?.Dispose();
            productCommentRipository?.Dispose();
        }

      













        #endregion
    }
}
