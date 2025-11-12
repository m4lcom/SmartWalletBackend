using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a9f692ac-c922-446c-95dd-d4ad84ced22e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e89356b7-4ae1-4cb3-a37a-b28b7c8243b0"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "Email", "Name", "PasswordHash", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5f91dd3b-d449-467b-9abe-821b3994af10"), true, new DateTime(2025, 11, 11, 18, 2, 48, 586, DateTimeKind.Utc).AddTicks(9417), "albus@mail.com", "Albus Percival Wulfrid Brian Dumbledore", "1234", 2, null },
                    { new Guid("6a363402-c1fa-40b3-91b9-5358a2857989"), true, new DateTime(2025, 11, 11, 18, 2, 48, 586, DateTimeKind.Utc).AddTicks(9423), "tom@mail.com", "Tom Marvolo Riddle", "1234", 2, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5f91dd3b-d449-467b-9abe-821b3994af10"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6a363402-c1fa-40b3-91b9-5358a2857989"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "Email", "Name", "PasswordHash", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a9f692ac-c922-446c-95dd-d4ad84ced22e"), true, new DateTime(2025, 11, 9, 20, 41, 15, 337, DateTimeKind.Utc).AddTicks(9416), "albus@mail.com", "Albus Dumbledore", "1234", 2, null },
                    { new Guid("e89356b7-4ae1-4cb3-a37a-b28b7c8243b0"), true, new DateTime(2025, 11, 9, 20, 41, 15, 337, DateTimeKind.Utc).AddTicks(9421), "admin@mail.com", "Admin Admin", "1234", 2, null }
                });
        }
    }
}
