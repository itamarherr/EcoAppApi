using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAdminPasswordAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "b142489c-4b09-4656-b5db-8c9f3bed91c0");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2b4f3496-7812-4386-89c3-19692115030d", "AQAAAAIAAYagAAAAEMe7OGAs/4Kqd4hIAfjULLyGnTmY5xm0eAhsrBr9buFnSmv0OwJKV0UR5U6BBuGkhQ==", "2660f796-ffb0-458d-8713-3e0d4e8634c2" });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2025, 1, 17, 20, 46, 9, 721, DateTimeKind.Utc).AddTicks(7810), new DateTime(2025, 1, 17, 22, 46, 9, 721, DateTimeKind.Local).AddTicks(7802) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2025, 1, 17, 20, 46, 9, 721, DateTimeKind.Utc).AddTicks(7815), new DateTime(2025, 1, 17, 22, 46, 9, 721, DateTimeKind.Local).AddTicks(7814) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "660384c3-5c68-46ef-85a2-1c97c62ecefd");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4e7f25b7-e3af-4314-adde-e895fb7562f2", "AQAAAAIAAYagAAAAEMsFo7nWvjDG40B8HfXcMK0ZDPE7V2e1Vpi+Uye/R1tKI2TRZBoEyyjyu54Vh7c7XA==", "7c4e4a0a-c5e9-43b0-a6cb-80ec804b4535" });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2025, 1, 17, 20, 41, 37, 206, DateTimeKind.Utc).AddTicks(2500), new DateTime(2025, 1, 17, 22, 41, 37, 206, DateTimeKind.Local).AddTicks(2498) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2025, 1, 17, 20, 41, 37, 206, DateTimeKind.Utc).AddTicks(2505), new DateTime(2025, 1, 17, 22, 41, 37, 206, DateTimeKind.Local).AddTicks(2504) });
        }
    }
}
