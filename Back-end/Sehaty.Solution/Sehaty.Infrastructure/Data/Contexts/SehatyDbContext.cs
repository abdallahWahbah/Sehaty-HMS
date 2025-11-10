using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Entities.User_Entities;
using Sehaty.Infrastructure.Data.Configrations;
using System.Reflection;

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

        }
    }
}
