using AngularEshop.Core.DTOs.Account;
using AngularEshop.DataLayer.Entities.Access;
using AngularEshop.DataLayer.Entities.Account;
using AngularEshop.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AngularEshop.Core.DTOs.Account.LoginUserDTO;

namespace AngularEshop.Core.Services.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task<List<User>> GetAllUsers();
        Task<RegisterUserResult> RegisterUser(RegisterUserDTO register);
        bool IsUserExistsByEmail(string email);
        Task<LoginUserResult> LoginUser(LoginUserDTO login, bool checkAdminRole = false);
        Task<User> GetUserByEmail(string Email);
        Task<User> GetUserById(long userId);
        Task<bool> ActivateUser(User user);
        Task<User> GetUserByEmailActiveCode(string emailActiveCode);
        Task EditUserInfo(EditUserDTO editUserDtO,long userId);
        bool IsUserAdmin(long userId);
      
     

    }
}
