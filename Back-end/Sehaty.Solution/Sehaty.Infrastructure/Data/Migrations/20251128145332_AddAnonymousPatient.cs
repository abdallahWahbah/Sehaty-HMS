using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sehaty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnonymousPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsActive", "LanguagePreference", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 999999, 0, "117e54a3-6136-4dd1-a627-67a606e9df88", "Anonymous@example.com", false, "Anonymous", true, "Arabic", "Anonymous", false, null, "ANONYMOUS@EXAMPLE.COM", "ANONYMOUS", "AQAAAAIAAYagAAAAEIuysQPnpCUd1/r5pfQHlWUtw+UQFAOemWVcGliWwnbvIrMr4GAFjdpgTCKV0k9EjA==", "+2000000000000", false, null, false, "Anonymous" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 3, 999999 });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Address", "Allergies", "BloodType", "ChrinicConditions", "DateOfBirth", "EmergencyContactName", "EmergencyContactPhone", "FirstName", "Gender", "LastName", "NationalId", "Patient_Id", "RegistrationDate", "Status", "UserId" },
                values: new object[] { 999999, "Unknown", "Unknown", "Unk", "Unknown", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unknown", "Unknown", "Anonymous", "Unknown", "Unknown", "Unknown", "PT-2025-0000", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Active", 999999 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 3, 999999 });

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 999999);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 999999);
        }
    }
}
