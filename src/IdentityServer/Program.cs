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
                var user = new ApplicationUser("5p95zp");
                var existUser = userManager.FindByNameAsync("5p95zp").GetAwaiter().GetResult();
                if (existUser == null)
                {
                    userManager.CreateAsync(user, "d^yz!*K4Pu@IbKuh").GetAwaiter().GetResult();
                    userManager.AddClaimAsync(user, new Claim("SuperAdmin", "GodMode"));
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
