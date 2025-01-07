using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminNotesToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "03b28b6b-1a73-44d4-9ab6-511ce4447243");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ae977b43-d6ee-4b94-8baa-e4b5570ef8df", "AQAAAAIAAYagAAAAEC17Og+eaDt3meeAxZKyM1XVlwQU0zkPKIhAZg81qIHeQAGZ7fIXpiwIbm27Ydu3Mg==", "fa140261-c046-48c2-bb0d-745df133855f" });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AdminNotes", "CreatedAt", "DateForConsultancy" },
                values: new object[] { null, new DateTime(2025, 1, 7, 20, 9, 27, 223, DateTimeKind.Utc).AddTicks(949), new DateTime(2025, 1, 7, 22, 9, 27, 223, DateTimeKind.Local).AddTicks(947) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AdminNotes", "CreatedAt", "DateForConsultancy" },
                values: new object[] { null, new DateTime(2025, 1, 7, 20, 9, 27, 223, DateTimeKind.Utc).AddTicks(954), new DateTime(2025, 1, 7, 22, 9, 27, 223, DateTimeKind.Local).AddTicks(953) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "29389cde-4f11-4a12-910b-99e67cdd1044");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e628bfef-73f8-48eb-a547-d8e307672d38", "AQAAAAIAAYagAAAAEAtYYj7pPrzK6l5okcDQ2dTPQxQy1lXh4U5IOMfAms92eNjjyrfkJxLQIwuOwB96AQ==", "8822372c-1897-4c24-8855-8b9122ebfa96" });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2024, 12, 25, 9, 14, 46, 502, DateTimeKind.Utc).AddTicks(4530), new DateTime(2024, 12, 25, 11, 14, 46, 502, DateTimeKind.Local).AddTicks(4527) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2024, 12, 25, 9, 14, 46, 502, DateTimeKind.Utc).AddTicks(4535), new DateTime(2024, 12, 25, 11, 14, 46, 502, DateTimeKind.Local).AddTicks(4534) });
        }
    }
}
