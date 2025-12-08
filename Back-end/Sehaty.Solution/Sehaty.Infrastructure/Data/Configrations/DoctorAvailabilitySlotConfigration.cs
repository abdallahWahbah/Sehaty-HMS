using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;

namespace Sehaty.Infrastructure.Data.Configrations
{
    internal class DoctorAvailabilitySlotConfigration : IEntityTypeConfiguration<DoctorAvailabilitySlot>
    {
        public void Configure(EntityTypeBuilder<DoctorAvailabilitySlot> builder)
        {
            builder.Property(d => d.Id).UseIdentityColumn(1, 1);
            builder.Property(d => d.StartTime).HasColumnType("time");
            builder.Property(d => d.EndTime).HasColumnType("time");
            builder.Property(d => d.Date).HasColumnType("date");
            builder.Property(d => d.DayOfWeek)
                   .HasConversion<int>();
            builder.HasIndex(d => new { d.DoctorId, d.Date, d.StartTime })
                .IsUnique();
            builder.HasOne(d => d.Doctor)
                   .WithMany(doc => doc.DoctorAvailabilitySlots)
                   .HasForeignKey(d => d.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.Property(d => d.DayOfWeek)
                .HasConversion(
                WD => WD.ToString(), // to DB
                WD => (WeekDays)Enum.Parse(typeof(WeekDays), WD))
                .HasMaxLength(100)
                .HasColumnName("WeekDays")
                .IsRequired();

            builder.HasIndex(d => new { d.DoctorId, d.Date, d.StartTime })
                   .IsUnique();
        }
    }
}
