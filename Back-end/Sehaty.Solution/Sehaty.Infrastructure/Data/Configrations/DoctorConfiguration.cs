namespace Sehaty.Infrastructure.Data.Configrations
{
    internal class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(D => D.Id);
            builder.Property(D => D.FirstName).IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            builder.Property(D => D.LastName).IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            builder.Property(D => D.Specialty).IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(D => D.LicenseNumber).IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            builder.Property(D => D.Qualifications).HasColumnType("nvarchar(max)");
            builder.Property(D => D.IsDeleted).HasDefaultValue(false);

            builder.HasMany(D => D.DoctorAvailabilitySlots)
                .WithOne(AS => AS.Doctor)
                .HasForeignKey(As => As.DoctorId);
            builder.HasMany(D => D.Prescriptions)
                .WithOne(Pr => Pr.Doctor)
                .HasForeignKey(Pr => Pr.DoctorId);

        }
    }
}
