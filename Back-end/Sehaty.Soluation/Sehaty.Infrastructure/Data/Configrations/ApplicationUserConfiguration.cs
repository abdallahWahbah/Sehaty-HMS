using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.User_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");
            builder.Property(n => n.UserName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            builder.HasIndex(n => n.UserName).IsUnique();
            builder.Property(n => n.FirstName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(n => n.LastName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            builder.HasIndex(n => n.Email).IsUnique();
            builder.Property(n => n.PhoneNumber)
                .HasColumnType("nvarchar")
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(u => u.LanguagePreference)
                   .HasConversion<string>();
            builder.Property(n => n.IsActive)
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .IsRequired();
            builder.Property(n => n.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");
            builder.Property(n => n.LastLogin)
                .HasColumnType("datetime");
            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
