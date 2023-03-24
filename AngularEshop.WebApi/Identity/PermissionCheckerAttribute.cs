using AngularEshop.Core.Services.Interfaces;
using AngularEshop.Core.Utilities.Common;
using AngularEshop.Core.Utilities.Extensions.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularEshop.WebApi.Identity
{
    public class PermissionCheckerAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public string role { get; set; }
        public IAccessService accessService { get; set; }
        public PermissionCheckerAttribute(string role)
        {
            this.role = role;
        }
        void IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext context)
        {
            accessService = (IAccessService)context.HttpContext.RequestServices
                .GetService(typeof(IAccessService));
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = context.HttpContext.User.GetUserId();
                var result = accessService.CheckUserRole(userId, this.role).Result;
                if (!result)
                {
                    context.Result = JsonResponseStatus.NoAccess();
                }
            }
            else
            {
                context.Result = JsonResponseStatus.NoAccess();
            }
        }
    }
}
