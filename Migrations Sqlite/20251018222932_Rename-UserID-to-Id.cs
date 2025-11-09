using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserIDtoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Users",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserID");
        }
    }
}
