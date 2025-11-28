using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;

namespace Sehaty.Infrastructure.Data.Contexts
{
    // It Will Be Modefied To Inherit From IdentityDbContext<ApplicationUser,IdentityRole> To Add Auth Tables 
    public class SehatyDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        // Main Constructor To Enable Dependancy Injection
        public SehatyDbContext(DbContextOptions<SehatyDbContext> options) : base(options) { }


        #region Database Tables

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<DoctorAvailabilitySlot> DoctorAvailabilitySlots { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<PrescriptionMedications> PrescriptionMedications { get; set; }
        public DbSet<AppointmentAuditLog> AppointmentAuditLogs { get; set; }
        public DbSet<MedicalRecordAuditLog> MedicalRecordAuditLogs { get; set; }
        public DbSet<DoctorAppointmentSlot> DoctorAppointmentSlots { get; set; }

        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Ignore<IdentityUserRole<int>>();

            // --------------------------
            // Custom User -> Role one-to-many
            // --------------------------
            //modelBuilder.Entity<ApplicationUser>()
            //    .HasOne(u => u.Role)
            //    .WithMany(r => r.Users)
            //    .HasForeignKey(u => u.RoleId)
            //    .OnDelete(DeleteBehavior.Restrict); // كل يوزر له رول واحد فقط

            // --------------------------
            // Apply your configurations from separate configuration classes
            // --------------------------
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

            var hasher = new PasswordHasher<ApplicationUser>();

            var anonymousUser = new ApplicationUser()
            {
                Id = 999999,
                CreatedAt = DateTime.MinValue,
                Email = "Anonymous@example.com",
                FirstName = "Anonymous",
                LastName = "Anonymous",
                IsActive = true,
                PhoneNumber = "+2000000000000",
                NormalizedEmail = "Anonymous@example.com".ToUpper(),
                LanguagePreference = LanguagePreferenceEnum.Arabic,
                UserName = "Anonymous",
                NormalizedUserName = "Anonymous".ToUpper(),
            };
            anonymousUser.PasswordHash = hasher.HashPassword(anonymousUser, "P@ssw0rd");

            modelBuilder.Entity<ApplicationUser>().HasData(anonymousUser);


            modelBuilder.Entity<Patient>().HasData(new Patient()
            {
                Id = 999999,
                Address = "Unknown",
                Allergies = "Unknown",
                BloodType = "Unk",
                ChrinicConditions = "Unknown",
                DateOfBirth = DateTime.MinValue,
                EmergencyContactName = "Unknown",
                EmergencyContactPhone = "Unknown",
                FirstName = "Anonymous",
                LastName = "Unknown",
                IsDeleted = false,
                NationalId = "Unknown",
                Patient_Id = "PT-2025-0000",
                Status = PatientStatus.Active,
                RegistrationDate = DateTime.MinValue,
                Gender = "Unknown",
                UserId = 999999
            });

            modelBuilder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>() { UserId = 999999, RoleId = 3 });

        }
    }
}
