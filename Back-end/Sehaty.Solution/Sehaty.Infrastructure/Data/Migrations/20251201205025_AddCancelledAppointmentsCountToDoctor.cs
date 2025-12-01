using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sehaty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelledAppointmentsCountToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DetectionPrice",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancelledAppointmentsCount",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 999999,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c5d2e5aa-6b09-455d-b4ac-dda9b9876ae4", "AQAAAAIAAYagAAAAED7kKyZZm/jcivOIPgKzhJwb3wpjIUyKF5DBzu/8xv9s6ve/qyRgZXvS3Vn71wEaMg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledAppointmentsCount",
                table: "Doctors");

            migrationBuilder.AlterColumn<int>(
                name: "DetectionPrice",
                table: "Doctors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 999999,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d3774de1-e6ec-416e-a79d-625fd54c31bc", "AQAAAAIAAYagAAAAEOSNljM1FcbJZK1IUUO4OJVmGBt1wNNzfLZ5qUJakDf0eIv5QFqUOW4p7TJEo/WJOQ==" });
        }
    }
}
