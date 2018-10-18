using eshopAPI.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace eshopAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    CreateRoles(services).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();


        static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            IConfiguration Configuration = serviceProvider.GetService<IConfiguration>();
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ShopUser>>();
            string[] roleNames = { UserRole.Admin.ToString(), UserRole.User.ToString(), UserRole.Blocked.ToString() };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //Here you could create a super user who will maintain the web app
            var poweruser = new ShopUser
            {
                UserName = Configuration["AdminUsername"],
                Email = Configuration["AdminUsername"],
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = Configuration["AdminUserPass"];
            var _user = await UserManager.FindByNameAsync(Configuration["AdminUsername"]);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                var confirmToken = await UserManager.GenerateEmailConfirmationTokenAsync(poweruser);
                await UserManager.ConfirmEmailAsync(poweruser, confirmToken);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, UserRole.Admin.ToString());
                }
            }
        }
    }
}
