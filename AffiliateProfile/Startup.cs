using System;
using AffiliateProfile.Utility;
using AutoMapper;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Repository;
using Repository.InterFace;
using Service;
using Service.EmailService;
using Stimulsoft.Base;

namespace AffiliateProfile
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(Configuration.GetConnectionString("Local")));

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            #region identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddRoleManager<RoleManager<IdentityRole>>()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddTransient<IUserClaimsPrincipalFactory<ApplicationUser>, MyClaimsPrincipalFactory>();

            services.Configure<IdentityOptions>(option =>
            {
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(1);  // for set time out cookie authentication
            });
            #endregion
            
            #region Gmail Service
            services.AddTransient<IEmailSender, EmailSender>(); // Dependecy injection For Email Service
            #endregion

            #region JetMailService
            services.AddTransient<IJetMailService, JetMailService>();
            #endregion
            #region AutoMapper
            services.AddAutoMapper(typeof(Startup));
            #endregion
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);//You can set Time   
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var contentRoot = env.ContentRootPath;
            var licenseFile = System.IO.Path.Combine(contentRoot, "Reports", "license.key");
            StiLicense.LoadFromFile(licenseFile);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseCors(builder => builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials());
            app.UseHttpsRedirection();

            app.UseStaticFiles();    

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
