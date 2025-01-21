using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

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
                columns: new[] { "ConcurrencyStamp", "FirstName", "ImageUrl", "LastName", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "aa6ed581-71fe-431e-9652-5cd270892b12", "Itamar", "https://i.pravatar.cc/66", "Herr", "AQAAAAIAAYagAAAAEA/re3cDidug0ezhdaIsDWNFueFkfsbfulUPXEnaksjr0ZKrzyEra7nQ8nzT7cPvlw==", "1234567890", "20c22678-8c38-4544-8ea1-b4c229542e50" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

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
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "ae977b43-d6ee-4b94-8baa-e4b5570ef8df", "AQAAAAIAAYagAAAAEC17Og+eaDt3meeAxZKyM1XVlwQU0zkPKIhAZg81qIHeQAGZ7fIXpiwIbm27Ydu3Mg==", null, "fa140261-c046-48c2-bb0d-745df133855f" });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2025, 1, 7, 20, 9, 27, 223, DateTimeKind.Utc).AddTicks(949), new DateTime(2025, 1, 7, 22, 9, 27, 223, DateTimeKind.Local).AddTicks(947) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DateForConsultancy" },
                values: new object[] { new DateTime(2025, 1, 7, 20, 9, 27, 223, DateTimeKind.Utc).AddTicks(954), new DateTime(2025, 1, 7, 22, 9, 27, 223, DateTimeKind.Local).AddTicks(953) });
        }
    }
}
