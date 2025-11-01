using Microsoft.EntityFrameworkCore;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using System.Reflection;

namespace Sehaty.Infrastructure.Data.Contexts
{
    // It Will Be Modefied To Inherit From IdentityDbContext<ApplicationUser,IdentityRole> To Add Auth Tables 
    public class SehatyDbContext : DbContext
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

        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
