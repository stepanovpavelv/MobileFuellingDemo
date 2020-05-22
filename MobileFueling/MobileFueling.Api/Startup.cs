using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MobileFueling.Api.Common.HealthChecks;
using MobileFueling.Api.Common.Localization;
using MobileFueling.DB;
using MobileFueling.Model;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace MobileFueling.Api
{
    public class Startup
    {
        private const string DEFAULT_VERSION = "v1";
        private const string DEFAULT_REQUEST_CULTURE = "ru";
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var sqlType = SetFuelContextOptions(services);

            // health checks
            var healthConnectionString = _configuration["ConnectionStrings:HealthConnection"];
            services.AddHealthChecks()
                .AddCheck("Database App", new SqlConnectionHealthCheck(healthConnectionString, sqlType));

            // очереди задач
            services.AddHangfire(c => c.UseMemoryStorage());

            // локализация
            services.AddTransient<IStringLocalizer, CustomStringLocalizer>();

            // аутентификация
            services.AddIdentity<ApplicationUser, ApplicationRole>(
                option =>
                {
                    option.User.RequireUniqueEmail = true;
                    option.Password.RequireDigit = false;
                    option.Password.RequiredLength = 5;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequireUppercase = false;
                    option.Password.RequireLowercase = false;
                }
            )
            .AddEntityFrameworkStores<FuelDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = _configuration["Jwt:Site"],
                    ValidIssuer = _configuration["Jwt:Site"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Add framework services.
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllers()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc(DEFAULT_VERSION, new OpenApiInfo
                {
                    Version = DEFAULT_VERSION,
                    Title = "MobileFuelling API",
                    Description = "Examples of MobileFuelling methods Web API"
                });

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id= "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                option.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, FuelDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHealthChecks("/healthcheck");

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire");

            var supportedCultures = new[]
            {
                new CultureInfo("ru"),
                new CultureInfo("en")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(DEFAULT_REQUEST_CULTURE),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseCors(builder =>
                builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(_configuration["SwaggerOptions:UIEndpoint"], _configuration["SwaggerOptions:Description"]);
                option.RoutePrefix = string.Empty;
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute("defaultapi", string.Concat("api/", DEFAULT_VERSION, "/{controller=Home}/{action=Index}/{id?}"));

            //    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

            //    endpoints.MapControllerRoute("userTypeRoute", string.Concat("api/", DEFAULT_VERSION, "/user/{userType}"));
            //});

            FuelInitializer.InitializePredefinedData(userManager, context);
        }

        private int SetFuelContextOptions(IServiceCollection services)
        {
            var connectionStr = _configuration.GetConnectionString("DefaultConnection");
            var sqltype = _configuration.GetSection("appSettings").GetValue<int>("SQlType");

            switch (sqltype)
            {
                case 0:
                    services.AddDbContext<FuelDbContext>(options =>
                            options.UseSqlServer(connectionStr));
                    break;
                case 1:
                    services.AddDbContext<FuelDbContext>(options =>
                            options.UseNpgsql(connectionStr));
                    break;
                default:
                    services.AddDbContext<FuelDbContext>(options =>
                            options.UseSqlServer(connectionStr));
                    break;
            }

            return sqltype;
        }
    }
}