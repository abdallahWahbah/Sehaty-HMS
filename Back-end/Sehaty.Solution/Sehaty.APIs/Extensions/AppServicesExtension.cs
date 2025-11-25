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
                    Title = "Sehaty",
                    Description = "Sehaty Medical Web API"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your valid token in the text input below . \r\n\r\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
                });

                swagger.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
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
                            Array.Empty<string>()
                        }
                    });
            });

            #endregion

            #region Add Services

            // Add UnitOfWork Class Injection
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Inject Service For Prescription To Dowmload Prescription
            services.AddScoped<IPrescriptionPdfService, PrescriptionPdfService>();

            // Inject Service For Doctor To Add And Manage Doctors
            //services.AddScoped<IDoctorService, DoctorService>();

            // Inject Service For Appointment To Add And Manage Appointments
            services.AddScoped<IAppointmentService, AppointmentService>();

            // Inject Service For Patient To Add And Manage Patient
            services.AddScoped<IPatientService, PatientService>();

            // Inject Service For File To Add And Manage Files
            services.AddScoped<IFileService, FileService>();
            // Inject Service For Billing To Add And Manage Billing

            services.AddScoped<IBillingService, BillingService>();
            services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();

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
            //Add email services
            services.AddTransient<IEmailSender, EmailSender>();
            //bind Twilio settings
            services.Configure<TwilioSettings>(configuration.GetSection("TwilioSMSSetting"));
            //Add SMS Service
            services.AddTransient<ISmsSender, SmsSender>();

            //add background service
            services.AddHostedService<AppointmentReminderService>();
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
