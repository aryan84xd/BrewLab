using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddBrewerAndGrinder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BrewerId",
                table: "Experiments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GrinderId",
                table: "Experiments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Brewers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrewMethodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brewers_BrewMethods_BrewMethodId",
                        column: x => x.BrewMethodId,
                        principalTable: "BrewMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Brewers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grinders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grinders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grinders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_BrewerId",
                table: "Experiments",
                column: "BrewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_GrinderId",
                table: "Experiments",
                column: "GrinderId");

            migrationBuilder.CreateIndex(
                name: "IX_Brewers_BrewMethodId",
                table: "Brewers",
                column: "BrewMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Brewers_UserId",
                table: "Brewers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Grinders_UserId",
                table: "Grinders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiments_Brewers_BrewerId",
                table: "Experiments",
                column: "BrewerId",
                principalTable: "Brewers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiments_Grinders_GrinderId",
                table: "Experiments",
                column: "GrinderId",
                principalTable: "Grinders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiments_Brewers_BrewerId",
                table: "Experiments");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiments_Grinders_GrinderId",
                table: "Experiments");

            migrationBuilder.DropTable(
                name: "Brewers");

            migrationBuilder.DropTable(
                name: "Grinders");

            migrationBuilder.DropIndex(
                name: "IX_Experiments_BrewerId",
                table: "Experiments");

            migrationBuilder.DropIndex(
                name: "IX_Experiments_GrinderId",
                table: "Experiments");

            migrationBuilder.DropColumn(
                name: "BrewerId",
                table: "Experiments");

            migrationBuilder.DropColumn(
                name: "GrinderId",
                table: "Experiments");
        }
    }
}
