namespace Sehaty.Infrastructure.Data.Configrations
{
    internal class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(P => P.Id);
            builder.Property(P => P.Patient_Id).IsRequired().HasColumnType("nvarchar(20)");
            builder.HasIndex(P => P.Patient_Id).IsUnique();
            builder.Property(P => P.FirstName).IsRequired().HasColumnType("nvarchar(50)");
            builder.Property(P => P.LastName).IsRequired().HasColumnType("nvarchar(50)");
            builder.Property(P => P.DateOfBirth).IsRequired();
            builder.Property(P => P.Gender).IsRequired().HasColumnType("nvarchar(10)");
            builder.Property(P => P.NationalId).HasColumnType("nvarchar(14)");
            builder.Property(P => P.BloodType).HasColumnType("nvarchar(3)");
            builder.Property(P => P.Allergies).HasColumnType("nvarchar(max)");
            builder.Property(P => P.ChrinicConditions).HasColumnType("nvarchar(max)");
            builder.Property(P => P.Address).HasColumnType("nvarchar(max)");
            builder.Property(P => P.EmergencyContactName).HasColumnType("nvarchar(100)");
            builder.Property(P => P.EmergencyContactPhone).HasColumnType("nvarchar(20)");
            builder.Property(P => P.Status).HasConversion<string>().HasColumnType("nvarchar(12)");
            builder.Property(P => P.IsDeleted).HasDefaultValue(false);

            builder.HasMany(P => P.Appointments)
                   .WithOne(A => A.Patient)
                   .HasForeignKey(A => A.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(P => P.Prescriptions)
                     .WithOne(PR => PR.Patient)
                     .HasForeignKey(PR => PR.PatientId)
                     .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(P => P.Billings)
                        .WithOne(B => B.Patient)
                        .HasForeignKey(B => B.PatientId)
                        .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(P => P.User)
                   .WithMany()
                   .HasForeignKey(P => P.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(P => P.MedicalRecord)
                   .WithOne(M => M.Patient)
                   .HasForeignKey<MedicalRecord>(M => M.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
