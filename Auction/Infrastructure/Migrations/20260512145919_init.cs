using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalId = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Poster = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ArchivalLots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemInfoId = table.Column<string>(type: "TEXT", nullable: true),
                    LotOwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndType = table.Column<int>(type: "INTEGER", nullable: false),
                    BoughtFor_Amount = table.Column<decimal>(type: "TEXT", nullable: true),
                    BoughtFor_Type = table.Column<int>(type: "INTEGER", nullable: true),
                    BuyerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CompletionTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivalLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivalLots_Items_ItemInfoId",
                        column: x => x.ItemInfoId,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArchivalLots_Users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArchivalLots_Users_LotOwnerId",
                        column: x => x.LotOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemInfoId = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    BuyoutPrice_Amount = table.Column<decimal>(type: "TEXT", nullable: true),
                    BuyoutPrice_Type = table.Column<int>(type: "INTEGER", nullable: true),
                    LotOwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentBet_BetParticipantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CurrentBet_BetAmount_Amount = table.Column<decimal>(type: "TEXT", nullable: true),
                    CurrentBet_BetAmount_Type = table.Column<int>(type: "INTEGER", nullable: true),
                    MinBetCurrency_Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    MinBetCurrency_Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lots_Items_ItemInfoId",
                        column: x => x.ItemInfoId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lots_Users_CurrentBet_BetParticipantId",
                        column: x => x.CurrentBet_BetParticipantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lots_Users_LotOwnerId",
                        column: x => x.LotOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivalLots_BuyerId",
                table: "ArchivalLots",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivalLots_ItemInfoId",
                table: "ArchivalLots",
                column: "ItemInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivalLots_LotOwnerId",
                table: "ArchivalLots",
                column: "LotOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_OwnerId",
                table: "Items",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_CurrentBet_BetParticipantId",
                table: "Lots",
                column: "CurrentBet_BetParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ItemInfoId",
                table: "Lots",
                column: "ItemInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_LotOwnerId",
                table: "Lots",
                column: "LotOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OriginalId",
                table: "Users",
                column: "OriginalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletCurrency_UserId",
                table: "WalletCurrency",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivalLots");

            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "WalletCurrency");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
