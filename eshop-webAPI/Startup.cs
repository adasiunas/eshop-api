using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using eshopAPI.DataAccess;
using eshopAPI.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using eshopAPI.Models;
using eshopAPI.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

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

            services.AddIdentity<ShopUser, IdentityRole>(opt => { opt.SignIn.RequireConfirmedEmail = true;})
                .AddEntityFrameworkStores<ShopContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "SecCookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.Domain = ".eshop-qa-api.azurewebsites.net";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(double.Parse(Configuration["CookieTimeSpan"]));
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "eshop-api", Version = "v1" });
            });

            services.AddAntiforgery(options => 
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.Domain = ".eshop-qa-api.azurewebsites.net";
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
            services.AddScoped<IShopUserRepository, ShopUserRepository>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc(opt => { opt.Filters.Add(typeof(ValidatorActionFilter)); })
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                
            }
            // TODO: remove this afterwards
            app.UseCors(
                options => options.AllowAnyMethod().AllowAnyHeader().AllowCredentials().AllowAnyOrigin()
            );
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "eshop-api V1");
            });
            
            loggerFactory.AddLog4Net();

            app.UseAuthentication();
            CreateRoles(serviceProvider).Wait();
            app.Use(next => context =>
            {
                string path = context.Request.Path.Value;
                if (path != null && path.StartsWith("/api"))
                {
                    var token = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("CSRF-TOKEN", token.RequestToken, new CookieOptions { HttpOnly = false, Domain = ".eshop-qa-api.azurewebsites.net" });
                }
                return next(context);
            });
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
