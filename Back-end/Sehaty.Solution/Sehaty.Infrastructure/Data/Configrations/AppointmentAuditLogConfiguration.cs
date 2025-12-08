namespace Sehaty.Infrastructure.Data.Configrations
{
    public class AppointmentAuditLogConfiguration : IEntityTypeConfiguration<AppointmentAuditLog>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AppointmentAuditLog> builder)
        {
            builder.Property(r => r.Action)
               .HasConversion<string>()
               .HasMaxLength(20)
               .HasColumnType("nvarchar(20)");
            builder.Property(r => r.ChangedBy)
              .HasConversion<string>()
              .HasMaxLength(20)
              .HasColumnType("nvarchar(20)");
        }
    }
}
