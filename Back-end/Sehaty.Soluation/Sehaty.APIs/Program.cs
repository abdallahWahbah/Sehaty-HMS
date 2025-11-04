using Microsoft.EntityFrameworkCore;
using Sehaty.Application.MappingProfiles;
using Sehaty.Application.Shared.AuthShared;
using Sehaty.Core.Entities.User_Entities;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Data.Contexts;
using Sehaty.Infrastructure.Data.SeedClass;
using Sehaty.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Sehaty.Application.Services.Contract.AuthService.Contract;
using Sehaty.Application.Services.IdentityService;


namespace Sehaty.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();

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
