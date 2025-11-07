using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entities.Business_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class MedicationConfigrations : IEntityTypeConfiguration<Medication>
    {
        public void Configure(EntityTypeBuilder<Medication> builder)
        {
            // Table name
            builder.ToTable("Medications");

            // Primary key
            builder.HasKey(m => m.Id);

            // Columns configuration
            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(100);





        }
    }
}
