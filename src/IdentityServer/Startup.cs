using IdentityServer.Data;
using IdentityServer.Describer;
using IdentityServer.Helpers;
using IdentityServer.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseInMemoryDatabase("Memory");
            });

            services.AddIdentity<IdentityUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireLowercase = false;

            }).AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders().AddErrorDescriber<CustomErrorDescriber>();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
            });
            services.AddAuthorization(option =>
            {
                option.AddPolicy("SuperAdmin", policy =>
                {
                    policy.RequireClaim("Admin", "true");
                });
            });
            services.AddIdentityServer()
                .AddInMemoryApiResources(Config.GetApis())
                .AddAspNetIdentity<IdentityUser>()
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddDeveloperSigningCredential();

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
            }

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
