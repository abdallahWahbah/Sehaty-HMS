using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entites;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace Sehaty.Infrastructure.Data.Configrations
{
    public class BillingConfiguration : IEntityTypeConfiguration<Billing>
    {
        public void Configure(EntityTypeBuilder<Billing> builder)
        {
            builder.ToTable("Billing");

            // Primary Key
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                   .ValueGeneratedOnAdd();

            // Basic Columns
            builder.Property(b => b.PatientId)
                   .IsRequired();

            builder.Property(b => b.AppointmentId)
                   .IsRequired();

            builder.Property(b => b.BillDate)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .IsRequired();

            builder.Property(b => b.Subtotal)
                   .HasColumnType("decimal(10,2)")
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(b => b.TaxAmount)
                   .HasColumnType("decimal(10,2)")
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(b => b.DiscountAmount)
                   .HasColumnType("decimal(10,2)")
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(b => b.TotalAmount)
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();


            // Enum Conversions
            builder.Property(b => b.Status)
                   .HasMaxLength(20)
                   .HasConversion(new EnumToStringConverter<BillingStatus>())
                   .IsRequired();

            builder.Property(b => b.PaymentMethod)
                   .HasMaxLength(30)
                   .HasConversion(new EnumToStringConverter<PaymentMethod>())
                   .IsRequired(false);


            // Nullable Fields
            builder.Property(b => b.PaidAmount)
                   .HasColumnType("decimal(10,2)")
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(b => b.PaidAt)
                   .HasColumnType("datetime")
                   .IsRequired(false);

            builder.Property(b => b.ItemsDetail)
                   .HasColumnType("varchar(max)")
                   .IsRequired(false);

            builder.Property(b => b.TransactionId)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(b => b.CommissionApplied)
                   .HasColumnType("decimal(5,2)")
                   .IsRequired(false);

            builder.Property(b => b.NetAmount)
                   .HasColumnType("decimal(10,2)")
                   .IsRequired(false);

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

            // Each appointment has exactly one billing record ? 
            builder.HasIndex(b => b.AppointmentId)
                   .IsUnique()
                   .HasDatabaseName("IX_Billing_AppointmentId_Unique");

        }
    }
}
