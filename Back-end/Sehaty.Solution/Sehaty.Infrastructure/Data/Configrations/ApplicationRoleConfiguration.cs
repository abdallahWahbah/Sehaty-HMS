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
