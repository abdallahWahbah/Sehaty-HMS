using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sehaty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCloumnPaymentLinkToBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentLink",
                table: "Billing",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 999999,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ce22826c-f5f6-4f5f-a517-5cbeac5ec280", "AQAAAAIAAYagAAAAEPzWdhfv8Qq4Pr4qwS61klph1nVz1VBiYHQg5eLNQ3kx8kjxCNsCuBz70muo+WXWTQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentLink",
                table: "Billing");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 999999,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "117e54a3-6136-4dd1-a627-67a606e9df88", "AQAAAAIAAYagAAAAEIuysQPnpCUd1/r5pfQHlWUtw+UQFAOemWVcGliWwnbvIrMr4GAFjdpgTCKV0k9EjA==" });
        }
    }
}
