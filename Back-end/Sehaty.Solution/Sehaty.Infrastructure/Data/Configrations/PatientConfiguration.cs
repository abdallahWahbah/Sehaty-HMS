using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entites;

namespace Sehaty.Infrastructure.Data.Configrations
{
    internal class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.PatientId).IsRequired().HasColumnType("nvarchar(20)");
            builder.HasIndex(p => p.PatientId).IsUnique();
            builder.Property(p => p.MRN).HasColumnType("nvarchar(20)");
            builder.Property(p => p.FirstName).IsRequired().HasColumnType("nvarchar(50)");
            builder.Property(p => p.LastName).IsRequired().HasColumnType("nvarchar(50)");
            builder.Property(p => p.DateOfBirth).IsRequired();
            builder.Property(p => p.Gender).IsRequired().HasColumnType("nvarchar(10)");
            builder.Property(p => p.NationalId).HasColumnType("nvarchar(14)");
            builder.Property(p => p.BloodType).HasColumnType("nvarchar(3)");
            builder.Property(p => p.Allergies).HasColumnType("nvarchar(max)");
            builder.Property(p => p.ChrinicConditions).HasColumnType("nvarchar(max)");
            builder.Property(p => p.Address).HasColumnType("nvarchar(max)");
            builder.Property(p => p.EmergencyContactName).HasColumnType("nvarchar(100)");
            builder.Property(p => p.EmergencyContactPhone).HasColumnType("nvarchar(20)");
            builder.Property(p => p.Status).HasConversion<string>().HasColumnType("nvarchar(12)");
        }
    }
}
