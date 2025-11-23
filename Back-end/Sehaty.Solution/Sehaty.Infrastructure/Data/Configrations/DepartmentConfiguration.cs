namespace Sehaty.Infrastructure.Data.Configrations
{
    internal class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd(); // Identity
            builder.Property(d => d.Name).IsRequired().HasColumnType("nvarchar(100)");
            builder.Property(d => d.NameLocal).HasColumnType("nvarchar(100)");
            builder.Property(d => d.Description).HasColumnType("nvarchar(max)");
        }
    }
}
