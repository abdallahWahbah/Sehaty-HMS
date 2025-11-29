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


            #region Add Payment Services

            services.Configure<PaymentSettings>(
                configuration.GetSection(nameof(PaymentSettings)));

            services.Configure<PaymobEgy2Settings>(
                configuration.GetSection(nameof(PaymobEgy2Settings)));

            #endregion

            #region Add AutoMapper Profiles

            // Add AutoMapper Profiles Injection
            // To Add Every Profile Automatically
            services.AddAutoMapper(cfg => { }, typeof(DoctorProfile).Assembly);

            #endregion

            #region Add Project Services

            // Add UnitOfWork Class Injection
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Inject Service For Prescription To Dowmload Prescription
            services.AddScoped<IPrescriptionPdfService, PrescriptionPdfService>();

            // Inject Service For Doctor To Add And Manage Doctors
            //services.AddScoped<IDoctorService, DoctorService>();

            // Inject Service For Appointment To Add And Manage Appointments
            services.AddScoped<IAppointmentService, AppointmentService>();

            // Inject Service For Notification To Add And Manage Notifications
            services.AddScoped<INotificationService, NotificationService>();

            // Inject Service For Patient To Add And Manage Patient
            services.AddScoped<IPatientService, PatientService>();

            // Inject Service For File To Add And Manage Files
            services.AddScoped<IFileService, FileService>();
            // Inject Service For Billing To Add And Manage Billing

            services.AddScoped<IBillingService, PaymobService>();

            // Inject Service For PaymentService To Add And Manage Billing
            services.AddScoped<IPaymentService, PaymentService>();

            //Add Email Service
            services.AddTransient<IEmailSender, EmailSender>();

            //Add SMS Service
            services.AddTransient<ISmsSender, SmsSender>();

            services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();

            //Add email services
            //bind Twilio settings
            services.Configure<TwilioSettings>(configuration.GetSection("TwilioSMSSetting"));


            //add background service
            services.AddHostedService<AppointmentReminderService>();
            services.AddHostedService<OldNotificationsCleanupService>();

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
