namespace Sehaty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredIndexForAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Doctor_AppointmentDateTime",
                table: "Appointments");

            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX IX_Doctor_AppointmentDateTime_Active
                    ON Appointments (DoctorId, AppointmentDateTime)
                    WHERE Status <> 'Canceled';"
                    );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Doctor_AppointmentDateTime",
                table: "Appointments");

            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Doctor_AppointmentDateTime_Active ON Appointments;");

        }
    }
}
