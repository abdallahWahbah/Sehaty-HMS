using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sehaty.Core.Entities.User_Entities;

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

            builder.HasMany(R => R.Users).WithOne()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
