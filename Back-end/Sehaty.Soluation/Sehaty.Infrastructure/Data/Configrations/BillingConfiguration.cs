using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entites;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class BillingConfiguration : IEntityTypeConfiguration<Billing>
    {
        public void Configure(EntityTypeBuilder<Billing> builder)
        {
            builder.ToTable("Billing");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(b => b.PatientId)
                   .IsRequired();

            builder.Property(b => b.AppointmentId)
                   .IsRequired();

            builder.Property(b => b.BillingDate)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .IsRequired();

            builder.Property(b => b.Amount)
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

            builder.Property(b => b.Status)
                   .HasMaxLength(20)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(b => b.PaymentMethod)
                   .HasMaxLength(30)
                   .HasConversion<string>()
                   .IsRequired(false);

            builder.Property(b => b.InvoiceNumber)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(b => b.Notes)
                   .HasColumnType("varchar(max)")
                   .IsRequired(false);

            // Relationships
            builder.HasOne(b => b.Patient)
                   .WithMany(p => p.Billings)
                   .HasForeignKey(b => b.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Appointment)
                   .WithMany(a => a.Billings)
                   .HasForeignKey(b => b.AppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Indexes / Constraints 
            builder.HasIndex(b => b.AppointmentId)
                   .IsUnique()
                   .HasDatabaseName("IX_Billing_AppointmentId_Unique");

            builder.HasIndex(b => b.InvoiceNumber)
                   .IsUnique()
                   .HasDatabaseName("IX_Billing_InvoiceNumber_Unique");

            builder.HasCheckConstraint(
                "CK_Billing_Status_Valid",
                "[Status] IN ('Pending','Paid','Overdue','Cancelled')"
            );

            builder.HasCheckConstraint(
                "CK_Billing_Amount_Positive",
                "[Amount] > 0"
            );
        }
    }
}
