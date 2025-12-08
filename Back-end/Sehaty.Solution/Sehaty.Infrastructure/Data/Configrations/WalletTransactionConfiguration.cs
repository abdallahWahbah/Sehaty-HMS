namespace Sehaty.Infrastructure.Data.Configrations
{
    internal class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasKey(W => W.Id);
            builder.Property(P => P.Amount).HasColumnType("decimal(10,2)");
            builder.Property(P => P.Type).HasConversion<string>();
            builder.Property(P => P.Notes).HasColumnType("nvarchar(max)");
            builder.Property(P => P.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");


            builder.HasOne<Patient>()
               .WithMany(P => P.WalletTransactions)
               .HasForeignKey(W => W.PatientId);
        }
    }
}
