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
    internal class DoctorAvailabilitySlotConfigration : IEntityTypeConfiguration<DoctorAvailabilitySlot>
    {
        public void Configure(EntityTypeBuilder<DoctorAvailabilitySlot> builder)
        {
            builder.Property(d => d.Id).UseIdentityColumn(1, 1);
            builder.Property(d => d.StartTime).HasColumnType("time");
            builder.Property(d => d.EndTime).HasColumnType("time");
            builder.Property(d => d.Date).HasColumnType("date");
        }
    }
}
