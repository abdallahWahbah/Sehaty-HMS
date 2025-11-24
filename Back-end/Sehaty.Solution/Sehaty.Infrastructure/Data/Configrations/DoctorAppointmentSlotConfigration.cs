using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class DoctorAppointmentSlotConfigration : IEntityTypeConfiguration<DoctorAppointmentSlot>
    {
        public void Configure(EntityTypeBuilder<DoctorAppointmentSlot> builder)
        {
            builder.HasOne(s => s.Appointment)
            .WithMany()
            .HasForeignKey(s => s.AppointmentId)
           .OnDelete(DeleteBehavior.SetNull);


        }
    }
}
