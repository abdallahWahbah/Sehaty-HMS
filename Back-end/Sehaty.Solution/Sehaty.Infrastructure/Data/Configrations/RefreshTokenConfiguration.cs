using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entities.User_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(256);
            builder.Property(t => t.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();
            builder.Property(t => t.Expires)
                .HasColumnType("datetime")
                .IsRequired();
            builder.HasOne(u => u.User)
                .WithMany(r => r.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
