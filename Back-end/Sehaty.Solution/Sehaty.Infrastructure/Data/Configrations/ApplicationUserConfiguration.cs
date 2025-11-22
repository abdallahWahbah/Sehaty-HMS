namespace Sehaty.Infrastructure.Data.Configrations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");
            builder.Property(n => n.UserName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            builder.HasIndex(n => n.UserName).IsUnique();
            builder.Property(n => n.FirstName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(n => n.LastName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            builder.HasIndex(n => n.Email).IsUnique();
            builder.Property(n => n.PhoneNumber)
                .HasColumnType("nvarchar")
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(u => u.LanguagePreference)
                   .HasConversion<string>();
            builder.Property(n => n.IsActive)
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .IsRequired();
            builder.Property(n => n.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(n => n.LastLogin)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");
            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId);
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(U => U.Role)
            //    .WithMany(R => R.Users)
            //    .HasForeignKey(U => U.RoleId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
