using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entities.User_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable("Roles");
            builder.Property(n => n.Name)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            builder.HasIndex(n => n.Name).IsUnique();
            builder.Property(n => n.Description)
                .HasColumnType("nvarchar(max)");
        }
    }
}
