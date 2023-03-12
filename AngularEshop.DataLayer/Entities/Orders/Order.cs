using AngularEshop.DataLayer.Entities.Account;
using AngularEshop.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularEshop.DataLayer.Entities.Orders
{
   public class Order : BaseEntity
    {
        #region properties
        public long UserId { get; set; }
        public bool IsPay { get; set; }
        public DateTime? PaymentDate { get; set; }
        #endregion
        #region relations
        public User User { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        #endregion
    }
}
