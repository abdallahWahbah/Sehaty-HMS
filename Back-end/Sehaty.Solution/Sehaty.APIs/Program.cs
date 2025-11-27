using Sehaty.Application.Services;
using Sehaty.Application.Services.Contract.BusinessServices.Contract;

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

            // Extension Method To Add All Services Needed To Run Application
            builder.Services.AddApplicationServices(builder.Configuration)
                .AddAuthServices(builder.Configuration)
                .AddErrorServices()
                .AddSwaggerServices();

            //builder.Services.AddScoped<IBillingService, BillingService>();

            builder.Services.Configure<PaymentSettings>(
                builder.Configuration.GetSection(nameof(PaymentSettings)));
            builder.Services.Configure<PaymobEgy2Settings>(
                builder.Configuration.GetSection(nameof(PaymobEgy2Settings)));


            builder.Services.AddHttpClient();



            var app = builder.Build();

            #region Update Database & Seed Data In Data Base 

            await app.ApplyMigrationWithSeed();

            #endregion

            // To Add Exception Our Custom Middleware To Handle Server Errors
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middleware To Handle All Errors Occur With Error Controller
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("AllowAngular");


            app.MapControllers();

            app.Run();
        }
    }
}
