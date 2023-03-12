using AngularEshop.Core.Services.Interfaces;
using AngularEshop.Core.Utilities.Common;
using AngularEshop.Core.Utilities.Extensions.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularEshop.WebApi.Controllers
{
  
    public class OrderController : SiteBaseController
    {
        #region constructor
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        #endregion
        #region add product to order
        [HttpGet("add-order")]
        public async Task<IActionResult> addProductToOrder(long productId,int count)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId =User.GetUserId();
                await orderService.AddProductToOrder(userId, productId, count);
                return JsonResponseStatus.Success(new {
                    details = await orderService.GetUserCartDetails(userId) });
            }

            return JsonResponseStatus.Error(new {message = "برای افزودن کالا ابتدا لاگین کنید"});
        }
        #endregion
        #region user cart details
        [HttpGet("get-order-details")]
        public async Task<IActionResult> GetUserCartDetails()
        {
            if (User.Identity.IsAuthenticated)
            {
                var details = await orderService.GetUserCartDetails(User.GetUserId());
                return JsonResponseStatus.Success(details);
            }
            return JsonResponseStatus.Error();
        }
        #endregion
        #region remove detail from cart
        [HttpGet("remove-order-detail/{detailId}")]
        public async Task<IActionResult>RemoveOrderDetail(int detailId )
        {
            if (User.Identity.IsAuthenticated)
            {
                var userOpenOrder = await orderService.GetUserOpenOrder(User.GetUserId());
               var detail = userOpenOrder.OrderDetails.SingleOrDefault(s => s.Id == detailId);
               if (detail != null)
                {
                   await orderService.DeleteOrderDetail(detail);
                    return JsonResponseStatus.Success(await orderService.GetUserCartDetails(User.GetUserId()));

                }


            }
            return JsonResponseStatus.Error();
        }
        #endregion
    }
}
