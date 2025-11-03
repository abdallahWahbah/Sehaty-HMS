using Microsoft.EntityFrameworkCore;
using Sehaty.Core.Entities.User_Entities;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Data.Contexts;
using Sehaty.Infrastructure.Data.SeedClass;
using Sehaty.Infrastructure.UnitOfWork;

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
