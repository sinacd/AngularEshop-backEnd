using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularEshop.Core.Utilities.Common
{
    public static class PathTools
    {
        #region domain

        public static string Domain = "https://localhost:44345";

        #endregion

        #region product

        public static string ProductImagePath = "/images/products/origin/";
        public static string ProductImageServerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products/origin/");

        #endregion


    }
}
