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
                var user = new ApplicationUser("mfa");
                var existUser = userManager.FindByNameAsync("mfa").GetAwaiter().GetResult();
                if (existUser == null)
                {
                    ApplicationRole role = new ApplicationRole("GodMode");
                    roleManager.CreateAsync(role).GetAwaiter().GetResult();
                    roleManager.AddClaimAsync(role, new Claim("SuperAdmin", "true")).GetAwaiter().GetResult();
                    userManager.CreateAsync(user, "kbr2626").GetAwaiter().GetResult();
                    var result = userManager.AddToRoleAsync(user, "GodMode").GetAwaiter().GetResult();
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
