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
    internal class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd(); // Identity
            builder.Property(d => d.Name).IsRequired().HasColumnType("nvarchar(100)");
            builder.Property(d => d.NameLocal).IsRequired().HasColumnType("nvarchar(100)");
            builder.Property(d => d.Description).IsRequired().HasColumnType("nvarchar(max)");
        }
    }
}
