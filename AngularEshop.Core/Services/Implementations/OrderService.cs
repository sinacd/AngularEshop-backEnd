using AngularEshop.Core.DTOs.Orders;
using AngularEshop.Core.Services.Interfaces;
using AngularEshop.DataLayer.Entities.Orders;
using AngularEshop.DataLayer.Ripository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularEshop.Core.Services.Implementations
{
   public class OrderService : IOrderService
    {

        #region constructor
        private readonly IGenericRipository<Order> orderRipository;
        private readonly IGenericRipository<OrderDetail> orderDetailRipository;
        private readonly IProductService productService;
        private readonly IUserService userService;

        public OrderService(IGenericRipository<Order> orderRipository,
            IGenericRipository<OrderDetail> orderDetailRipository,
            IProductService productService,
          IUserService userService
            )
        {
            this.orderRipository = orderRipository;
            this.orderDetailRipository = orderDetailRipository;
            this.productService = productService;
            this.userService = userService;
        }

   

        #endregion
        #region order
        public async Task<Order> CreateUserOrder(long userId)
        {
            var order = new Order
            {
                UserId = userId
            };
            await orderRipository.AddEntity(order);
            await orderRipository.SaveChanges();
            return order;
        }
        public async Task<Order> GetUserOpenOrder(long userId)
        {
            var order = await orderRipository.GetEntitiesQuery()
                .Include(s=>s.OrderDetails).ThenInclude(s=>s.Product)
                .SingleOrDefaultAsync(s=>s.UserId== userId && !s.IsPay);
            if (order== null)
            {
                order = await CreateUserOrder(userId);
               
            }
            return order;
       
        }



        #endregion




        #region orderDetail
        public async Task AddProductToOrder(long userId, long productId, int count)
        {
            var user = await userService.GetUserById(userId)  ;
            var product = await productService.GetProductForUserOrder(productId)  ;
            if (user!=null &&product!=null)
            {
                var order = await GetUserOpenOrder(userId);


                if (count<1)
                {
                    count = 1;
                }
                var details = await GetOrderDetails(order.Id);
                var existsDetail = details.SingleOrDefault(s => s.ProductId == productId);

                if (existsDetail !=null&& !existsDetail.IsDelete)
                {
                    existsDetail.Count += count;
                     orderDetailRipository.UpdateEntity(existsDetail);
                }
                else
                    if(existsDetail != null && existsDetail.IsDelete)
                {
                    existsDetail.IsDelete = false;
                    existsDetail.Price = product.Price;
                    existsDetail.Count = 1;
                    orderDetailRipository.UpdateEntity(existsDetail);
                }
                else
                {
                    var detail = new OrderDetail
                    {
                      
                        OrderId = order.Id,
                        ProductId = productId,
                        Count = count,
                        Price = product.Price

                    };
                    await orderDetailRipository.AddEntity(detail);

                }

               
              
                await orderDetailRipository.SaveChanges();
            }
           
        }

        public async Task<List<OrderDetail>> GetOrderDetails(long orderId)
        {
            return await orderDetailRipository.GetEntitiesQuery().Where(s => s.OrderId == orderId).ToListAsync();
        }
        public async Task<List<OrderCartDetail>> GetUserCartDetails(long userId)
        {
            var openOrder = await GetUserOpenOrder(userId);
            if (openOrder == null)
            {
                return null;
            }
            return openOrder.OrderDetails.Where(s => !s.IsDelete).Select(f => new OrderCartDetail
            {
                Id =f.Id,
                Count = f.Count,
                Price = f.Price,
                Title = f.Product.ProductName,
                ImageName = f.Product.ImageName
            }).ToList();

        }
        public async Task DeleteOrderDetail(OrderDetail detail)
        {
            if (detail.Count == 1)
            {
                orderDetailRipository.RemoveEntity(detail);
                await orderDetailRipository.SaveChanges();
            }
            else
            {
                detail.Count--;
                orderDetailRipository.UpdateEntity(detail);
                await orderDetailRipository.SaveChanges();
            }
        }


        #endregion



        #region disposable
        public void Dispose()
        {
            orderRipository.Dispose();
            orderDetailRipository.Dispose();
        }

      




        #endregion

    }
}
