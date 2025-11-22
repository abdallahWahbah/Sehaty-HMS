namespace Sehaty.Infrastructure.Data.Configrations
{
    public class MedicalRecordConfigration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.Property(m => m.Id)
                   .UseIdentityColumn(1, 1);

            builder.Property(m => m.Temperature)
                   .HasColumnType("decimal(4,1)");

            builder.Property(m => m.Weight)
                   .HasColumnType("decimal(5,1)");

            builder.Property(r => r.RecordType)
                .HasConversion<string>()
                .HasDefaultValue(RecordType.Diagnosis)
                .HasMaxLength(20)
                .HasColumnType("nvarchar(20)");

            builder.Property(m => m.Symptoms)
                .HasColumnType("nvarchar(max)");

            builder.Property(m => m.Diagnosis)
                .HasColumnType("nvarchar(max)");

            builder.Property(m => m.Notes)
                .HasColumnType("nvarchar(max)");

            builder.Property(m => m.VitalBp)
                .HasColumnType("nvarchar(20)");

            builder.HasMany(m => m.Prescriptions)
                   .WithOne(p => p.MedicalRecord)
                   .HasForeignKey(p => p.MedicalRecordId);

            builder.HasOne(m => m.Appointment)
                   .WithMany(a => a.MedicalRecords)
                   .HasForeignKey(m => m.AppointmentId);
        }

    }
}
