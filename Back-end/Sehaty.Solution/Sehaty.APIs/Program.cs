using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sehaty.Application.MappingProfiles;
using Sehaty.Application.Services.Contract.AuthService.Contract;
using Sehaty.Application.Services.IdentityService;
using Sehaty.Application.Shared.AuthShared;
using Sehaty.Core.Entities.User_Entities;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Data.Contexts;
using Sehaty.Infrastructure.Data.SeedClass;
using Sehaty.Infrastructure.UnitOfWork;
using System.Text;
using System.Text.Json.Serialization;


namespace Sehaty.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // Allows enum values to be read/written as strings instead of numbers
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();

            #region Swagger Setting
            builder.Services.AddSwaggerGen(swagger =>
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
            // Add DbContext Class Injection
            builder.Services.AddDbContext<SehatyDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Sehaty"));
            });

            // Add Identity Class Injection
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<SehatyDbContext>();

            // Add UnitOfWork Class Injection
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add AutoMapper Profiles Injection
            // To Add Every Profile Automatically
            builder.Services.AddAutoMapper(cfg => { }, typeof(DoctorProfile).Assembly);
            builder.Services.AddAutoMapper(cfg => { }, typeof(DoctorAvailabilitySlotProfile).Assembly);


            //builder.Services.AddAutoMapper(cfg => { }, typeof(AppointmentProfile).Assembly);

            // Instead Of Writeing Every Profile Like This :

            //builder.Services.AddAutoMapper(cfg =>
            //{
            //    cfg.AddProfile<MedicalRecordProfile>();
            //    cfg.AddProfile<DoctorProfile>();

            //}, typeof(MedicalRecordProfile).Assembly);

            //Add Jwt Authentication
            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
            builder.Services.AddAuthentication(options =>
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });


            #endregion

            var app = builder.Build();
            #region Seed Data In Data Base
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
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
