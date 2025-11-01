using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Infrastructure.Data.Configrations
{
	public class NotificationConfigrations : IEntityTypeConfiguration<Notification>
	{
		public void Configure(EntityTypeBuilder<Notification> builder)
		{
			builder.ToTable("Notifications");  //table name
			builder.HasKey(n => n.Id); //primary key
			builder.Property(n => n.Id).ValueGeneratedOnAdd(); //identity
			builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
			builder.Property(n => n.Message).IsRequired();
			builder.Property(n => n.CreatedAt).IsRequired();
			builder.Property(n => n.CreatedAt)
				   .HasDefaultValueSql("GETDATE()");
			builder.HasOne(n => n.User)
				   .WithMany(u => u.Notification)
				   .HasForeignKey(n => n.UserId)
				   .OnDelete(DeleteBehavior.Cascade);
			builder.Property(n => n.NotificationType)
				   .HasConversion<string>()  
				   .IsRequired();

		}
	}
}
