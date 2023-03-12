using AngularEshop.DataLayer.context;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularEshop.Core.Utilities.Extensions.Connection
{
    public static class ConnectionExtension
    {
        public static IServiceCollection AddApllicationDbContext(this IServiceCollection service,
            IConfiguration configuration)
        {
            service.AddDbContext<AngularEshopDbContext>(options =>
            {
                var connectionstring = "ConnectionStrings:AngularEshopConnection:Development";
                options.UseSqlServer(configuration[connectionstring]);
            });
            return service;
        }
    }
}
