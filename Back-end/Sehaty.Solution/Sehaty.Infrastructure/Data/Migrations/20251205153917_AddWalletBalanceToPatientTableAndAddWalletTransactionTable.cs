namespace Sehaty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletBalanceToPatientTableAndAddWalletTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Departments_DepartmentId",
                table: "Doctors");

            migrationBuilder.AddColumn<decimal>(
               name: "WalletBalance",
               table: "Patients",
               type: "decimal(10,2)",
               nullable: false,
               defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int",nullable: false)
                        .Annotation("SqlServer:Identity","1, 1"),
                    PatientId = table.Column<int>(type: "int",nullable: false),
                    PatientId1 = table.Column<int>(type: "int",nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(10,2)",nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)",nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)",nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)",nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime",nullable: false,defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions",x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Patients_PatientId1",
                        column: x => x.PatientId1,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_PatientId",
                table: "WalletTransactions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_PatientId1",
                table: "WalletTransactions",
                column: "PatientId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Departments_DepartmentId",
                table: "Doctors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Departments_DepartmentId",
                table: "Doctors");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "WalletBalance",
                table: "Patients");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Doctors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Departments_DepartmentId",
                table: "Doctors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
