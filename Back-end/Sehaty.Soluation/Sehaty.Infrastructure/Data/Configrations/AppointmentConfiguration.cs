using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sehaty.Core.Entites;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.Property(a => a.PatientId).IsRequired();
            builder.Property(a => a.DoctorId).IsRequired();
            builder.Property(a => a.AppointmentDateTime).IsRequired();

            builder.Property(a => a.DurationMinutes)
                   .IsRequired()
                   .HasDefaultValue(30);

            builder.Property(a => a.ReasonForVisit)
                   .HasColumnType("varchar(max)");

            builder.Property(a => a.Status)
                   .IsRequired()
                   .HasMaxLength(20)
                   .HasDefaultValue("Pending");

            builder.Property(a => a.ScheduledDate)
                   .HasColumnType("date");

            builder.Property(a => a.ScheduledTime)
                   .HasColumnType("time");

            builder.Property(a => a.BookingDateTime)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(a => a.ConfirmationDateTime)
                   .HasColumnType("datetime");

            builder.Property(a => a.CancellationReason)
                   .HasMaxLength(255);

            // Relationships
            builder.HasOne(a => a.Patient)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Doctor)
                   .WithMany(d => d.Appointments)
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes / Constraints
            builder.HasIndex(a => new { a.DoctorId, a.AppointmentDateTime })
                   .IsUnique()
                   .HasDatabaseName("IX_Doctor_AppointmentDateTime");

            builder.HasCheckConstraint("CK_Appointments_DurationMinutes_Positive", "[DurationMinutes] > 0");

            builder.HasCheckConstraint(
                "CK_Appointments_Status_Valid",
                "[Status] IN ('Pending','Confirmed','In Progress','Completed','Canceled','No-Show','Emergency')"
            );
        }
    }
}
