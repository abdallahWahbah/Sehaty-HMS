using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sehaty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnPriceToDoctorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Doctors",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Doctors");
        }
    }
}
