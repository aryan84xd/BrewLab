using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrewLab.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coffees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Roast = table.Column<string>(type: "text", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: true),
                    TastingNotes = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coffees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coffees_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Experiments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BrewMethod = table.Column<string>(type: "text", nullable: true),
                    CoffeeWeight = table.Column<decimal>(type: "numeric", nullable: false),
                    WaterWeight = table.Column<decimal>(type: "numeric", nullable: false),
                    BrewTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Remark = table.Column<string>(type: "text", nullable: true),
                    Aroma = table.Column<int>(type: "integer", nullable: false),
                    Acidity = table.Column<int>(type: "integer", nullable: false),
                    Body = table.Column<int>(type: "integer", nullable: false),
                    Overall = table.Column<int>(type: "integer", nullable: false),
                    CoffeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experiments_Coffees_CoffeeId",
                        column: x => x.CoffeeId,
                        principalTable: "Coffees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Experiments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coffees_UserId",
                table: "Coffees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_CoffeeId",
                table: "Experiments",
                column: "CoffeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_UserId",
                table: "Experiments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Experiments");

            migrationBuilder.DropTable(
                name: "Coffees");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
