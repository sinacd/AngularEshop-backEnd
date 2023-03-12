using AngularEshop.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngularEshop.Core.DTOs.Orders;

using AngularEshop.DataLayer.Entities.Orders;
using AngularEshop.DataLayer.Ripository;
using Microsoft.EntityFrameworkCore;
using AngularEshop.DataLayer.Entities.Access;
using AngularEshop.DataLayer.Entities.Account;

namespace AngularEshop.Core.Services.Implementations
{
    public class AccessService : IAccessService
    {
        #region constructor
        private readonly IGenericRipository<Role> roleRipository;
        private readonly IGenericRipository<UserRole> userRoleRipository;
        private readonly IGenericRipository<User> userRipository;
        public AccessService(IGenericRipository<Role> roleRipository,
            IGenericRipository<UserRole> userRoleRipository,
            IGenericRipository<User> userRipository)
        {
            this.roleRipository = roleRipository;
            this.userRoleRipository = userRoleRipository;
            this.userRipository = userRipository;
        }
        #endregion
        #region user role
        public async Task<bool> CheckUserRole(long userId, string role)
        {
            return await userRoleRipository.GetEntitiesQuery().AsQueryable().AnyAsync
                (s=>s.UserId==userId&&s.Role.Name==role);
        }
        #endregion
        #region dispose
        public void Dispose()
        {
            roleRipository?.Dispose();
            userRoleRipository?.Dispose();
            userRipository?.Dispose();
        }
        #endregion

    }
}
