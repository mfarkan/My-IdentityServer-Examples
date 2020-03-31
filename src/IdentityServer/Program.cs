using IdentityServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider
                    .GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var user = new ApplicationUser("admin");
                var existUser = userManager.FindByNameAsync("admin").GetAwaiter().GetResult();
                if (existUser == null)
                {
                    ApplicationRole role = new ApplicationRole("CourierManageGodMode");
                    roleManager.CreateAsync(role).GetAwaiter().GetResult();
                    userManager.CreateAsync(user, "kubra2626").GetAwaiter().GetResult();
                    var result = userManager.AddToRoleAsync(user, "CourierManageGodMode").GetAwaiter().GetResult();
                }
                //userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
                //userManager.AddClaimAsync(user, new Claim("basic.claim", "big.cookie")).GetAwaiter().GetResult();
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
