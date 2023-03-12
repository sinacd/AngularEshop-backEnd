using AngularEshop.Core.Security;
using AngularEshop.Core.Services.Implementations;
using AngularEshop.Core.Services.Interfaces;
using AngularEshop.Core.Services.Utilities;
using AngularEshop.Core.Utilities.Extensions.Connection;
using AngularEshop.DataLayer.Ripository;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace AngularEshop.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostingEnviroment)
        {
            Configuration = configuration;
            HostingEnviroment = hostingEnviroment;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment HostingEnviroment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region configure appsetting file
            services.AddControllersWithViews();
            services.AddSingleton<IConfiguration>(
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json")
                .Build()
                );
            #endregion

            #region smtp server
            var from = Configuration.GetSection("Mail")["From"];
            var gmailSender = Configuration.GetSection("Gmail")["Sender"];
            var gmailPassword = Configuration.GetSection("Gmail")["Password"];
            var gmailPort = Convert.ToInt32(Configuration.GetSection("Gmail")["Port"]);

            services.AddFluentEmail(gmailSender, from)
                .AddRazorRenderer()
                .AddSmtpSender(new SmtpClient("smtp.gmail.com")
                {
                    UseDefaultCredentials = false,
                    Port = gmailPort,
                    Credentials = new NetworkCredential(gmailSender, gmailPassword),
                    EnableSsl = true
                });
            services.AddScoped<IMailSender, MailSender>();
            #endregion

            #region Add DbContext
            services.AddApllicationDbContext(Configuration);
            services.AddScoped(typeof(IGenericRipository<>), typeof(GenericRipository<>));
            #endregion

            #region ApplicationServices
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISliderService, SliderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPasswordHelper, PasswordHelper>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IAccessService, AccessService>();
            //     services.AddScoped<IMailSender, SendEmail>();
            //   services.AddScoped<IViewRenderService,RenderViewToString>();



            #endregion

            #region Authentication

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://localhost:44345",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AngularEshopJwtBearer"))
                    };
                });

            #endregion

            #region Cors
            services.AddCors(options =>
            {
                options.AddPolicy("EnableCors", Builder =>
                {
                    Builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()

                        .Build();
                });
            });
            #endregion

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AngularEshop.WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AngularEshop.WebApi v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("EnableCors");
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
