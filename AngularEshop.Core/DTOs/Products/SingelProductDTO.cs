using System;


namespace AngularEshop.Core.DTOs.Products
{
    public class SingelProductDTO
    {
        #region properties
        public long ProductId { get; set; }

        public string ImageName { get; set; }
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        #endregion
       
    }
}
