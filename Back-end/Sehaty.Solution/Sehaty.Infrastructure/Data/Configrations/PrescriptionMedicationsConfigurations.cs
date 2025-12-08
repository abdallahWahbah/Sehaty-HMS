namespace Sehaty.Infrastructure.Data.Configrations
{
    public class PrescriptionMedicationsConfigurations : IEntityTypeConfiguration<PrescriptionMedications>
    {
        public void Configure(EntityTypeBuilder<PrescriptionMedications> builder)
        {
            builder.HasKey(pm => pm.Id);
            builder.Property(m => m.Dosage)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.Frequency)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.Duration)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.HasIndex(pm => new { pm.PrescriptionId, pm.MedicationId }).IsUnique();
        }
    }
}
