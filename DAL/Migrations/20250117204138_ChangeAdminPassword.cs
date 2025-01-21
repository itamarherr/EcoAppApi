using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "f1c90189-47e8-482f-85db-a9d353d9a269");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aa6ed581-71fe-431e-9652-5cd270892b12", "AQAAAAIAAYagAAAAEA/re3cDidug0ezhdaIsDWNFueFkfsbfulUPXEnaksjr0ZKrzyEra7nQ8nzT7cPvlw==", "20c22678-8c38-4544-8ea1-b4c229542e50" });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2025, 1, 17, 0, 10, 45, 261, DateTimeKind.Utc).AddTicks(1550), new DateTime(2025, 1, 17, 2, 10, 45, 261, DateTimeKind.Local).AddTicks(1547) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2025, 1, 17, 0, 10, 45, 261, DateTimeKind.Utc).AddTicks(1555), new DateTime(2025, 1, 17, 2, 10, 45, 261, DateTimeKind.Local).AddTicks(1553) });
        }
    }
}
