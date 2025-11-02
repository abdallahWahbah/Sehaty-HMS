using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entites;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class MedicalRecordConfigration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.Property(m => m.Id).UseIdentityColumn(1, 1);
            builder.Property(m => m.Temperature).HasColumnType("decimal(4,1)");
            builder.Property(m => m.Weight).HasColumnType("decimal(4,1)");
            builder.Property(r => r.RecordType).HasConversion<string>()
            .HasDefaultValue(RecordType.Diagnosis).HasMaxLength(20).HasColumnType("nvarchar(20)");
        }
    }
}
