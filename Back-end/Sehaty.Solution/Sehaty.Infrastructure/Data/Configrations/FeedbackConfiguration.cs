namespace Sehaty.Infrastructure.Data.Configrations
{
    internal class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.HasKey(F => F.Id);
            builder.Property(F => F.Comments).HasColumnType("nvarchar").HasMaxLength(500);
            builder.HasOne(F => F.Appointment)
                .WithMany(A => A.Feedbacks)
                .HasForeignKey(F => F.AppointmentId);
        }
    }
}
