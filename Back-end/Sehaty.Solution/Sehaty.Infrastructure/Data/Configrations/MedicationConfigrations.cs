namespace Sehaty.Infrastructure.Data.Configrations
{
    public class MedicationConfigrations : IEntityTypeConfiguration<Medication>
    {
        public void Configure(EntityTypeBuilder<Medication> builder)
        {
            // Table name
            builder.ToTable("Medications");

            // Primary key
            builder.HasKey(m => m.Id);


            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(100);





        }
    }
}
