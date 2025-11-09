using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seeddedatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "Email", "Name", "PasswordHash", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a9f692ac-c922-446c-95dd-d4ad84ced22e"), true, new DateTime(2025, 11, 9, 20, 41, 15, 337, DateTimeKind.Utc).AddTicks(9416), "albus@mail.com", "Albus Dumbledore", "1234", 2, null },
                    { new Guid("e89356b7-4ae1-4cb3-a37a-b28b7c8243b0"), true, new DateTime(2025, 11, 9, 20, 41, 15, 337, DateTimeKind.Utc).AddTicks(9421), "admin@mail.com", "Admin Admin", "1234", 2, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a9f692ac-c922-446c-95dd-d4ad84ced22e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e89356b7-4ae1-4cb3-a37a-b28b7c8243b0"));
        }
    }
}
