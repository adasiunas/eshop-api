﻿using System;
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
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using eshopAPI.Utils;
using eshopAPI.Models.ViewModels;
using eshopAPI.Utils.Export;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using eshopAPI.Utils.Import;
using eshopAPI.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eshopAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ShopContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("EshopConnection")));

            services.AddIdentity<ShopUser, IdentityRole>(opt => { opt.SignIn.RequireConfirmedEmail = true; })
                .AddEntityFrameworkStores<ShopContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(double.Parse(Configuration["CookieTimeSpan"]));
                options.SlidingExpiration = true;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var errorResponse = JsonConvert.SerializeObject(new ErrorResponse(ErrorReasons.Unauthorized, "Unauthorized"));
                    return context.Response.WriteAsync(errorResponse);
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    var errorResponse = JsonConvert.SerializeObject(new ErrorResponse(ErrorReasons.Forbidden, "Access forbidden"));
                    return context.Response.WriteAsync(errorResponse);
                };
            });
            services.AddCors();

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "eshop-api", Version = "v1" });
            });

            services.Configure<SecurityStampValidatorOptions>(o =>
                o.ValidationInterval = TimeSpan.FromSeconds(0));

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
            services.AddScoped<IImageCloudService>(provider =>
            {
                IImageCloudService cloudService = new ImageCloudService(provider.GetRequiredService<ILogger<IImageCloudService>>(), Configuration);
                foreach(string decoratorType in Configuration.GetSection("ImageCloudDecorators").GetChildren().Select(x => x.Value).ToArray())
                {
                    switch (decoratorType)
                    {
                        case "LOCAL":
                            cloudService = new ImageLocalStorageDecorator(cloudService, provider.GetRequiredService<IHostingEnvironment>());
                            break;
                    }
                }
                return cloudService;
            });
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IUserFeedbackRepository, UserFeedbackRepository>();
            services.AddScoped<IImportService, ExcelImportService>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<IDiscountService, DiscountService>();


            if (Configuration["ExportFile"] == "CSV")
                services.AddScoped<IExportService, CsvExportService>();
            else
                services.AddScoped<IExportService, ExportService>();

            services.AddSingleton(typeof(AntiforgeryMiddleware));
            services.AddSingleton(typeof(LoggingMiddleware));

            services.AddOData();
            
            // this is needed so that swagger would work with odata-created links
            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });

            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(ValidatorActionFilter));
                opt.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                opt.Filters.Add(new ExceptionsHandlingFilter());
            })
            .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddMvc()
                .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                
            }
            // TODO: remove this afterwards
            app.UseCors(
                options => options.WithOrigins(new string[] 
                {
                    "http://eshop-qa-web.azurewebsites.net",
                    "https://eshop-qa-web.azurewebsites.net",
                    "http://localhost:3000",
                    "http://127.0.0.1:3000"
                }).WithExposedHeaders("X-CSRF-COOKIE")
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials()
            );

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "eshop-api V1");
            });
            
            loggerFactory.AddLog4Net(x => !x.StartsWith("Microsoft.EntityFrameworkCore"));

            app.UseAuthentication();
            app.UseAntiforgeryMiddleware();
            app.UseLoggingMiddleware();
            app.UseMvc();

            var builder = new ODataConventionModelBuilder();
            builder.EnableLowerCamelCase();

            builder.EntitySet<UserVM>("Users").EntityType.HasKey(e => e.Id);
            builder.EntitySet<ItemVM>("Items").EntityType.HasKey(e => e.ID);
            builder.EntitySet<AdminItemVM>("AdminItems").EntityType.HasKey(e => e.ID);
            builder.EntitySet<AdminOrderVM>("AdminOrders").EntityType.HasKey(e => e.ID);
            builder.EntitySet<OrderVM>("Orders").EntityType.HasKey(e => e.ID);
            builder.EntitySet<UserFeedbackVM>("AdminFeedback").EntityType.HasKey(e => e.ID);
            builder.EntitySet<AdminDiscountVM>("Discount").EntityType.HasKey(e => e.ID);
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.MapODataServiceRoute("api/odata", "api/odata", builder.GetEdmModel());
                routeBuilder.Select().Expand().Filter().OrderBy().MaxTop(1000).Count();
                // Work-around for #1175
                routeBuilder.EnableDependencyInjection();
            });
        }

    }
}
