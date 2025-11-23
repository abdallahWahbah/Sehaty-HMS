namespace Sehaty.Infrastructure.Data.Configrations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments", T =>
            T.HasCheckConstraint("CK_Appointments_DurationMinutes_Positive", "[DurationMinutes] > 0"));

            builder.Property(a => a.PatientId).IsRequired();
            builder.Property(a => a.DoctorId).IsRequired();
            builder.Property(a => a.AppointmentDateTime).IsRequired();

            builder.Property(a => a.DurationMinutes)
                   .IsRequired()
                   .HasDefaultValue(30);

            builder.Property(a => a.ReasonForVisit)
                   .HasColumnType("varchar(max)");


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



            builder.Property(A => A.Status)
               .HasConversion<string>()
               .HasDefaultValue(AppointmentStatus.Pending)
               .HasColumnType("nvarchar(max)");
        }
    }
}
