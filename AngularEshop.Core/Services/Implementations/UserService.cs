using AngularEshop.Core.DTOs.Account;
using AngularEshop.Core.DTOs.Orders;
using AngularEshop.Core.Security;
using AngularEshop.Core.Services.Interfaces;
using AngularEshop.Core.Services.Utilities;
using AngularEshop.DataLayer.Entities.Access;
using AngularEshop.DataLayer.Entities.Account;
using AngularEshop.DataLayer.Entities.Common;
using AngularEshop.DataLayer.Ripository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AngularEshop.Core.DTOs.Account.LoginUserDTO;


namespace AngularEshop.Core.Services.Implementations
{
    public class UserService : IUserService
    {
        #region constructor
        private IGenericRipository<User> userRipository;
        private IGenericRipository<UserRole> userRoleRipository;
        private IPasswordHelper passwordHelper;
        //     private IMailSender mailSender;
        //     private IViewRenderService renderView;
        private readonly IMailSender _mailSender;

       
        public UserService(IGenericRipository<User> userRipository, IPasswordHelper passwordHelper
        , IMailSender mailSender, IGenericRipository<UserRole> userRoleRipository
       )
        {
            this.userRipository = userRipository;
            this.userRoleRipository = userRoleRipository;
            this.passwordHelper = passwordHelper;
            //      this.mailSender = mailSender;
            //     this.renderView = renderView;
            _mailSender = mailSender;

        }
        #endregion

        #region User Section
      public async Task<List<User>> GetAllUsers()
        {
            return await userRipository.GetEntitiesQuery().ToListAsync();
        }
        public async Task<RegisterUserResult> RegisterUser(RegisterUserDTO register)
        {
            if (IsUserExistsByEmail(register.Email))
                return RegisterUserResult.EmailExists;
            var user = new User
            {
                Email=register.Email.SanitizeText(),
                Address=register.Address.SanitizeText(),
                FirstName=register.FirstName.SanitizeText(),
                LastName=register.LastName.SanitizeText(),
                EmailActiveCode=Guid.NewGuid().ToString(),
                Password= passwordHelper.EncodePasswordMd5(register.Password)
            };
            await userRipository.AddEntity(user);
            await userRipository.SaveChanges();

                 string body = $"{Directory.GetCurrentDirectory()}/emails/sample.cshtml";
            _mailSender.SendHtmlGmail(user, "فروشگاه اینترنتی clothy",body);

            //mailSender.Send("sipodaw195@probdd.com", "ثبت نام فروشگاه لباسی",body);
            return RegisterUserResult.Success; 
        }
        public bool IsUserExistsByEmail(string email)
        {
            return userRipository.GetEntitiesQuery().Any(s => s.Email == email.ToLower().Trim());
        }
        public async Task<LoginUserDTO.LoginUserResult> LoginUser(LoginUserDTO login, bool checkAdminRole = false)
        {
            var password = passwordHelper.EncodePasswordMd5(login.Password);

            var user = await userRipository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.Email == login.Email.ToLower().Trim() && s.Password == password);
           
            if (user == null) return LoginUserResult.IncorrectData;

            if (!user.Isactivated) return LoginUserResult.NotActivated;

            if (checkAdminRole)
            {
                if (!IsUserAdmin(user.Id))
                {
                    return LoginUserResult.NotAdmin;
                }
            }

            return LoginUserResult.Success;
        }


        public async Task<User> GetUserByEmail(string email)
        {
            return await userRipository.GetEntitiesQuery().SingleOrDefaultAsync(s => s.Email == email.ToLower().Trim());
        }
        public async Task<User> GetUserById(long userId)
        {
            return await userRipository.GetEntityById(userId);
        }
        public async Task<User> GetUserByEmailActiveCode(string emailActiveCode)
        {
            return await userRipository.GetEntitiesQuery().SingleOrDefaultAsync(s=>s.EmailActiveCode== emailActiveCode);
        }
        public async Task<bool> ActivateUser(User user)
        {
            user.Isactivated = true;
            user.EmailActiveCode = Guid.NewGuid().ToString();
            userRipository.UpdateEntity(user);
          await  userRipository.SaveChanges();
            return true;
        }
        public async Task EditUserInfo(EditUserDTO editUserDtO, long id)
        {
            var mainUser = await userRipository.GetEntityById(id);
            if (mainUser!=null)
            {
                mainUser.FirstName = editUserDtO.FirstName;
                mainUser.LastName = editUserDtO.LastName;
                mainUser.Address = editUserDtO.Address;
                userRipository.UpdateEntity(mainUser);
                await userRipository.SaveChanges();
            }

        }



        #endregion
        #region Admin Section

        public bool IsUserAdmin(long userId)
        {
          var on = userRoleRipository.GetEntitiesQuery().Include(s => s.Role).Where(f => f.Role.Name == "Admin" && f.UserId== userId).Select(f => new BaseEntity
            {
                Id = f.UserId,
            }).SingleOrDefault();
            if (on !=null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region dispose
        public void Dispose()
        {
            this.userRipository?.Dispose();
            this.userRoleRipository?.Dispose();
        }
     
















        #endregion
    }
}
