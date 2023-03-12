using AngularEshop.Core.DTOs.Account;
using AngularEshop.Core.Services.Interfaces;
using AngularEshop.Core.Utilities.Common;
using AngularEshop.Core.Utilities.Extensions.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static AngularEshop.Core.DTOs.Account.LoginUserDTO;

namespace AngularEshop.WebApi.Controllers
{

    public class AccountController : SiteBaseController
    {
        #region constructor
        private IUserService userService;
        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }
        #endregion
        #region register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO register)
        {
            if (ModelState.IsValid)
            {
                var res = await userService.RegisterUser(register);

                switch (res)
                {
                    case RegisterUserResult.EmailExists: return JsonResponseStatus.Error(new { status = "EmailExist" });

                }
            }
            return JsonResponseStatus.Success();
        }
        #endregion
        #region login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO login)
        {

            if (ModelState.IsValid)
            {
                var res = await userService.LoginUser(login);

                switch (res)
                {
                    case LoginUserResult.IncorrectData: return JsonResponseStatus.NotFound(new { message = "  کاربری با این مشخصات وجود ندارد  " });
                    case LoginUserResult.NotActivated: return JsonResponseStatus.Error(new { message = " حساب کابری شما فعال نشده است" });


                    case LoginUserResult.Success:
                        var user = await userService.GetUserByEmail(login.Email);
                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AngularEshopJwtBearer"));
                        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                        var tokenOptions = new JwtSecurityToken(
                            issuer: "https://localhost:44345",
                            claims: new List<Claim>
                            {
                                new Claim(ClaimTypes.Name,user.Email),
                                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
                            },
                            expires: DateTime.Now.AddDays(30),
                            signingCredentials: signinCredentials   

                            );
                        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);


                        return JsonResponseStatus.Success(new { token = tokenString,
                            expireTime = 30,
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            userId = user.Id,
                            address = user.Address });

                }

            }
            return JsonResponseStatus.Error();
        }
        #endregion
        #region authentication
        [HttpPost("check-auth")]
        public async Task<IActionResult> CheckUserAuth()
        {
            if(User.Identity.IsAuthenticated)
            {
               
               var user= await userService.GetUserById(User.GetUserId());
                return JsonResponseStatus.Success(new
                {
                    userId = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    address = user.Address,
                    email = user.Email
                }) ;
            }
            return JsonResponseStatus.Error();
        }
        #endregion
        #region Acttive user account
        [HttpGet("activate-user/{id}")]
        public async Task<IActionResult> ActivateAccount(string id)
        {
            var user = await userService.GetUserByEmailActiveCode(id);
            if (user != null)
            {
                var flag = await userService.ActivateUser(user);
                if (flag==true)
                {
                    return JsonResponseStatus.Success();
                }
                return JsonResponseStatus.NotFound(new { message = "اکتیو با شکست مواجه شد" });
            }
            return JsonResponseStatus.NotFound();
        }
        #endregion
        #region signOut
        [HttpGet("sign-out")]
        public async Task<IActionResult> LogOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync();
                return JsonResponseStatus.Success();
            }
            return JsonResponseStatus.Error();
        }


        #endregion
        #region edit user
        [HttpPost("EditUser")]
        public async Task<IActionResult> EditUser([FromBody] EditUserDTO editUser)
        {
            if (User.Identity.IsAuthenticated)
            {
               await userService.EditUserInfo(editUser, User.GetUserId());
                return JsonResponseStatus.Success(new  { message="اطلاعات کاربر با موفقیت ویرایش شد" });
            }
            return JsonResponseStatus.UnAuthorized();
        }
        #endregion
    }
}

