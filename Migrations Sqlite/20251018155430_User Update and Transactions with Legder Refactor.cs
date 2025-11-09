using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdateandTransactionswithLegderRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLedgers_Wallets_DestinationWalletId",
                table: "TransactionLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLedgers_Wallets_SourceWalletId",
                table: "TransactionLedgers");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserID",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLedgers_DestinationWalletId",
                table: "TransactionLedgers");

            migrationBuilder.DropColumn(
                name: "DestinationTransactionId",
                table: "TransactionLedgers");

            migrationBuilder.DropColumn(
                name: "DestinationWalletId",
                table: "TransactionLedgers");

            migrationBuilder.RenameColumn(
                name: "SourceWalletId",
                table: "TransactionLedgers",
                newName: "WalletId2");

            migrationBuilder.RenameColumn(
                name: "SourceTransactionId",
                table: "TransactionLedgers",
                newName: "WalletId1");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionLedgers_SourceWalletId",
                table: "TransactionLedgers",
                newName: "IX_TransactionLedgers_WalletId2");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "TransactionLedgers",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WalletId",
                table: "TransactionLedgers",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserID",
                table: "Wallets",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLedgers_WalletId",
                table: "TransactionLedgers",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLedgers_WalletId1",
                table: "TransactionLedgers",
                column: "WalletId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLedgers_Wallets_WalletId",
                table: "TransactionLedgers",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLedgers_Wallets_WalletId1",
                table: "TransactionLedgers",
                column: "WalletId1",
                principalTable: "Wallets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLedgers_Wallets_WalletId2",
                table: "TransactionLedgers",
                column: "WalletId2",
                principalTable: "Wallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLedgers_Wallets_WalletId",
                table: "TransactionLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLedgers_Wallets_WalletId1",
                table: "TransactionLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLedgers_Wallets_WalletId2",
                table: "TransactionLedgers");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserID",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLedgers_WalletId",
                table: "TransactionLedgers");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLedgers_WalletId1",
                table: "TransactionLedgers");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "TransactionLedgers");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "TransactionLedgers");

            migrationBuilder.RenameColumn(
                name: "WalletId2",
                table: "TransactionLedgers",
                newName: "SourceWalletId");

            migrationBuilder.RenameColumn(
                name: "WalletId1",
                table: "TransactionLedgers",
                newName: "SourceTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionLedgers_WalletId2",
                table: "TransactionLedgers",
                newName: "IX_TransactionLedgers_SourceWalletId");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationTransactionId",
                table: "TransactionLedgers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationWalletId",
                table: "TransactionLedgers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserID",
                table: "Wallets",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLedgers_DestinationWalletId",
                table: "TransactionLedgers",
                column: "DestinationWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLedgers_Wallets_DestinationWalletId",
                table: "TransactionLedgers",
                column: "DestinationWalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLedgers_Wallets_SourceWalletId",
                table: "TransactionLedgers",
                column: "SourceWalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
