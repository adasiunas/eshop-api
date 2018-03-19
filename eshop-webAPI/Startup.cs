using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using log4net.Core;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;
using eshopAPI.DataAccess;
using eshopAPI.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using eshopAPI.Models;
using eshopAPI.Services;

namespace eshopAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddDbContext<ShopContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("EshopConnection")));

            services.AddIdentity<ShopUser, IdentityRole>(opt => { opt.SignIn.RequireConfirmedEmail = true; })
                .AddEntityFrameworkStores<ShopContext>()
                .AddDefaultTokenProviders();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "eshop-api", Version = "v1" });
            });

            // register services for DI
            // AddTransient - creates new services for every injection
            // AddScoped - creates and uses same service during request
            // AddSingleton - creates when first time requested and uses same instance all time

            // register data access layer
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAttributeRepository, AttributeRepository>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton(Configuration);

            services.AddMvc(opt => { opt.Filters.Add(typeof(ValidatorActionFilter)); })
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseCors(
                    options => options.WithOrigins("http://localhost:3000").AllowAnyMethod()
                    );
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "eshop-api V1");
                });
            }
            loggerFactory.AddLog4Net();
            app.UseAuthentication();
            CreateRoles(serviceProvider).Wait();
            app.UseMvc();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
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
                Email = Configuration["AdminUserPass"],
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = Configuration["AdminUserPass"];
            var _user = await UserManager.FindByNameAsync(Configuration["AdminUsername"]);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, UserRole.Admin.ToString());

                }
            }
        }
    }
}
