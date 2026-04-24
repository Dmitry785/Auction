using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class @fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BuyoutPrice_Amount",
                table: "Lots",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuyoutPrice_Type",
                table: "Lots",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBet_BetAmount_Amount",
                table: "Lots",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentBet_BetAmount_Type",
                table: "Lots",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentBet_BetParticipantId",
                table: "Lots",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Lots",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "ItemInfoId",
                table: "Lots",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MinBetCurrency_Amount",
                table: "Lots",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MinBetCurrency_Type",
                table: "Lots",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Lots",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Lots",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalId = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletCurrency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletCurrency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletCurrency_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lots_CurrentBet_BetParticipantId",
                table: "Lots",
                column: "CurrentBet_BetParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ItemInfoId",
                table: "Lots",
                column: "ItemInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_OwnerId",
                table: "Lots",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletCurrency_UserId",
                table: "WalletCurrency",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_Items_ItemInfoId",
                table: "Lots",
                column: "ItemInfoId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_Users_CurrentBet_BetParticipantId",
                table: "Lots",
                column: "CurrentBet_BetParticipantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_Users_OwnerId",
                table: "Lots",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lots_Items_ItemInfoId",
                table: "Lots");

            migrationBuilder.DropForeignKey(
                name: "FK_Lots_Users_CurrentBet_BetParticipantId",
                table: "Lots");

            migrationBuilder.DropForeignKey(
                name: "FK_Lots_Users_OwnerId",
                table: "Lots");

            migrationBuilder.DropTable(
                name: "WalletCurrency");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Lots_CurrentBet_BetParticipantId",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_ItemInfoId",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_OwnerId",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "BuyoutPrice_Amount",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "BuyoutPrice_Type",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "CurrentBet_BetAmount_Amount",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "CurrentBet_BetAmount_Type",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "CurrentBet_BetParticipantId",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "ItemInfoId",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "MinBetCurrency_Amount",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "MinBetCurrency_Type",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Lots");
        }
    }
}
