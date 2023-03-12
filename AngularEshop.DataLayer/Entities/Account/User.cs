using AngularEshop.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AngularEshop.DataLayer.Entities.Access;
using AngularEshop.DataLayer.Entities.Product;
using AngularEshop.DataLayer.Entities.Orders;

namespace AngularEshop.DataLayer.Entities.Account
{
   public class User : BaseEntity
    {
        #region properties
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage ="لطفا {0} را وارد کنید")]
        [MaxLength(100,ErrorMessage ="تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string Email { get; set; }
        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string Password { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string LastName { get; set; }
        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string Address { get; set; }
      
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string EmailActiveCode { get; set; }
        public bool Isactivated { get; set; }
        #endregion
        #region relations
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<ProductComment> ProductComments { get; set; }

        public ICollection<Order> Orders { get; set; }
        #endregion
    }
}
