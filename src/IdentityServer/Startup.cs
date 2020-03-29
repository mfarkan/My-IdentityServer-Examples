using IdentityServer.Data;
using IdentityServer.Describer;
using IdentityServer.Helpers;
using IdentityServer.Models;
using IdentityServer.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            var migrationAssembly = typeof(Startup).Assembly.GetName().Name;

            services.AddDbContext<UserManagementDbContext>(config =>
            {
                config.UseNpgsql(Configuration.GetConnectionString("IdServerConnection"));
                //config.UseInMemoryDatabase("Memory");
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<UserManagementDbContext>()
                .AddDefaultTokenProviders().AddErrorDescriber<CustomErrorDescriber>();
            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/LogOut";
                config.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            });
            services.AddAuthorization(option =>
            {
                option.AddPolicy("SuperAdmin", policy =>
                {
                    policy.RequireClaim("SuperAdmin", "true");
                });
            });
            services.AddIdentityServer()
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddConfigurationStore(options =>
                    {
                        options.DefaultSchema = "public";
                        options.ConfigureDbContext = db =>
                         {
                             db.UseNpgsql(Configuration.GetConnectionString("IdServerConnection"), sql =>
                              {
                                  sql.MigrationsAssembly(migrationAssembly);
                              });
                         };
                    }).AddOperationalStore(options =>
                    {
                        options.DefaultSchema = "public";
                        options.ConfigureDbContext = db =>
                         {
                             db.UseNpgsql(Configuration.GetConnectionString("IdServerConnection"), sql =>
                             {
                                 sql.MigrationsAssembly(migrationAssembly);
                             });
                         };
                    })
                    .AddDeveloperSigningCredential();
            //services.AddIdentityServer()
            //    .AddInMemoryApiResources(Config.GetApis())
            //    .AddAspNetIdentity<IdentityUser>()
            //    .AddInMemoryClients(Config.GetClients())
            //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
            services.AddControllersWithViews(option =>
            {
                option.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                option.Filters.Add(new SecurityHeadersAttribute());
            }
            ).AddDataAnnotationsLocalization(o =>
            {
                o.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    return factory.Create(typeof(SharedResource));
                };
            }).AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix);
            services.AddLocalization(o =>
            {
                o.ResourcesPath = "Resources";
            });
            services.AddAntiforgery(option => option.HeaderName = "X-XSRF-Token");
            services.AddHttpContextAccessor();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            var supportedCultures = new List<CultureInfo> { new CultureInfo("tr-TR"), new CultureInfo("en-US") };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("tr-TR"),
                SupportedUICultures = supportedCultures,
                SupportedCultures = supportedCultures,
                RequestCultureProviders = new[] { new CookieRequestCultureProvider() },
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            // middleware added to startup.
            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
