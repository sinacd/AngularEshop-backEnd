using AngularEshop.Core.DTOs.Products;
using AngularEshop.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularEshop.Core.Services.Interfaces
{
   public interface IProductService:IDisposable
    {
        #region product
        Task AddProduct(Product product);
        Task UpdateProduct(Product product);
        Task<FilterProductsDTO> FilterProducts(FilterProductsDTO filter);
        Task<Product> GetProductById(long productId);
        Task<bool> IsExistsProductById(long productId);
        Task<List<Product>> GetRelatedProducts(long productId);
        Task<Product> GetProductForUserOrder(long productId);

        #region admin edit
        Task<EditProductDTO> GetProductForEdit(long productId);
        Task EditProduct(EditProductDTO product);
        #endregion



        #endregion
        #region product categories
        Task<List<ProductCategory>> GetAllActiveProductCategories();
        #endregion
        #region product gallery
        Task<List<SingelProductDTO>> GetProductActiveGalleries(long productId);
        #endregion
        #region product comments

        Task<List<ProductCommentDTO>> GetActiveProductComments(long productId);
        Task<ProductCommentDTO> AddProductComment(AddProductCommentDTO comment, long userId);
        #endregion

    }
}
