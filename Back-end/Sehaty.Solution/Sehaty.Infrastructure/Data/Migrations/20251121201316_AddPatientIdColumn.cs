using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sehaty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientId",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Patients");
        }
    }
}
