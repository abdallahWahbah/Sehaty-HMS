using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sehaty.Infrastructure.Data.Migraions
{
    /// <inheritdoc />
    public partial class EditDateInDoctorAviliablityMakeNullabe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorAvailabilitySlots_DoctorId_Date_StartTime",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "DoctorAvailabilitySlots",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAvailabilitySlots_DoctorId_Date_StartTime",
                table: "DoctorAvailabilitySlots",
                columns: new[] { "DoctorId", "Date", "StartTime" },
                unique: true,
                filter: "[Date] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorAvailabilitySlots_DoctorId_Date_StartTime",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "DoctorAvailabilitySlots",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAvailabilitySlots_DoctorId_Date_StartTime",
                table: "DoctorAvailabilitySlots",
                columns: new[] { "DoctorId", "Date", "StartTime" },
                unique: true);
        }
    }
}
