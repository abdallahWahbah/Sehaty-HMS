using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;

namespace Sehaty.Infrastructure.Data.Configrations
{
    internal class PrescriptionConfigrations : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");  //table name
            builder.HasKey(n => n.Id); //primary key
            builder.Property(n => n.Id).ValueGeneratedOnAdd(); //identity
            builder.Property(p => p.Status)//store in db as string
                   .HasDefaultValue(PrescriptionStatus.Active)
                   .HasConversion<string>();

            builder.Property(p => p.MedicationName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Dosage)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Frequency)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(p => p.Duration)
                   .HasMaxLength(100);
            builder.Property(p => p.DigitalSignature)
                   .HasMaxLength(255);

            builder.Property(p => p.SpecialInstructions)
                   .HasMaxLength(200);
            builder.Property(p => p.DateIssued)
                  .IsRequired()
                  .HasDefaultValueSql("GETDATE()"); //set default value to current date

            builder.HasOne(p => p.Appointment) //set foreign key to appointment
                   .WithMany(a => a.Prescriptions)
                   .HasForeignKey(p => p.AppointmentId)
                   .OnDelete(DeleteBehavior.Cascade);// Cascade on delete

            builder.HasOne(p => p.MedicalRecord) //set foreign key to medical record
                  .WithMany(mr => mr.Prescriptions)
                  .HasForeignKey(p => p.RecordId);
            builder.Property(p => p.RecordId)
                  .IsRequired(false);

            builder.HasOne(p => p.Patient) //set foreign key to patient
                   .WithMany(pt => pt.Prescriptions)
                   .HasForeignKey(p => p.PatientId)
                   .OnDelete(DeleteBehavior.SetNull);// Set null on delete
            builder.Property(p => p.PatientId)
                   .IsRequired(false);

            builder.HasOne(p => p.Doctor) //set foreign key to doctor
                  .WithMany(d => d.Prescriptions)
                  .HasForeignKey(p => p.DoctorId);
            builder.Property(p => p.DoctorId)
                   .IsRequired(false);
        }
    }
}

