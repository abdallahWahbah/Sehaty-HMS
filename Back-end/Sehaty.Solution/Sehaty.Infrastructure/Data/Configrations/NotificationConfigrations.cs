using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entites;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class NotificationConfigrations : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");  //table name
            builder.HasKey(n => n.Id); //primary key
            builder.Property(n => n.Id).ValueGeneratedOnAdd(); //identity
            builder.Property(n => n.Title).IsRequired().HasMaxLength(255);
            builder.Property(n => n.Message).IsRequired();

            builder.Property(n => n.Priority)
                   .IsRequired()
                   .HasDefaultValue(NotificationPriority.Normal)
                   .HasMaxLength(10)
                   .HasConversion<string>(); // store enum as string

            builder.Property(n => n.IsRead)
                   .HasDefaultValue(false)
                   .IsRequired();
            builder.Property(n => n.ReadAt)
                   .IsRequired(false);

            builder.Property(n => n.SentViaSMS)
                   .HasDefaultValue(false)
                   .IsRequired();
            builder.Property(n => n.SentViaEmail)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(n => n.RelatedEntityType)
               .HasMaxLength(50)
               .IsRequired(false);

            builder.Property(n => n.RelatedEntityId)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(n => n.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .IsRequired();

            builder.HasOne(n => n.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(n => n.NotificationType)
                   .HasConversion<string>()
                   .IsRequired();

        }
    }
}
