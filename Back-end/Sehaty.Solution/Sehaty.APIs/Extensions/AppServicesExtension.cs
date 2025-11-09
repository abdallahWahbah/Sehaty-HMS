using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sehaty.APIs.Errors;
using Sehaty.Application.MappingProfiles;
using Sehaty.Application.Services.Contract.AuthService.Contract;
using Sehaty.Application.Services.IdentityService;
using Sehaty.Application.Services.PDFservice;
using Sehaty.Application.Shared.AuthShared;
using Sehaty.Core.Entities.User_Entities;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Data.Contexts;
using Sehaty.Infrastructure.Data.SeedClass;
using Sehaty.Infrastructure.Service.Email;
using Sehaty.Infrastructure.UnitOfWork;
using System.Text;
namespace Sehaty.APIs.Extensions
{
    public static class AppServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
        {

            // Add DbContext Class Injection
            services.AddDbContext<SehatyDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Sehaty"));
            });


            #region Add Authentications Services

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleManagementService, RoleManagementService>();
            services.AddTransient<IEmailSender, EmailSender>();
            // Add Identity Class Injection
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<SehatyDbContext>();

            #endregion

            #region Swagger Setting

            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Sehaty Web Api",
                    Description = "The Sehaty Medical Web API"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                    });
            });

            #endregion

            #region Add Services

            // Add UnitOfWork Class Injection
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Inject Service For Prescription To Dowmload Prescription
            services.AddScoped<PrescriptionPdfService>();

            // Add DbContext Class Injection
            services.AddDbContext<SehatyDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Sehaty"));
            });

            // Add AutoMapper Profiles Injection
            // To Add Every Profile Automatically
            services.AddAutoMapper(cfg => { }, typeof(DoctorProfile).Assembly);

            //Add Jwt Authentication
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()!;
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });


            #endregion


            #region Validation Error Configuration
            services.Configure<ApiBehaviorOptions>(cfg =>
            {
                cfg.InvalidModelStateResponseFactory = (context) =>
                {
                    var errors = context.ModelState
                    .Where(P => P.Value!.Errors.Count > 0)
                    .SelectMany(P => P.Value!.Errors)
                    .Select(E => E.ErrorMessage);

                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });
            #endregion


            return services;
        }

        public static async Task ApplyMigrationWithSeed(this WebApplication app)
        {
            #region Update Database & Seed Data In Data Base 
            using var scoped = app.Services.CreateScope();
            var services = scoped.ServiceProvider;
            var dbContext = services.GetRequiredService<SehatyDbContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                await dbContext.Database.MigrateAsync();
                await dbContext.SeedDataAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error has occured during migration !!!");
            }

            #endregion
        }
    }
}
