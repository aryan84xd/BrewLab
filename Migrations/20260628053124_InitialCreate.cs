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
                name: "BrewMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrewParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrewMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataType = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrewParameters_BrewMethods_BrewMethodId",
                        column: x => x.BrewMethodId,
                        principalTable: "BrewMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coffees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Roast = table.Column<int>(type: "integer", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: true),
                    TastingNotes = table.Column<string>(type: "text", nullable: true),
                    RoastDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Process = table.Column<int>(type: "integer", nullable: true),
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
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CoffeeWeight = table.Column<decimal>(type: "numeric", nullable: false),
                    WaterWeight = table.Column<decimal>(type: "numeric", nullable: false),
                    WaterTemperature = table.Column<decimal>(type: "numeric", nullable: true),
                    GrindSetting = table.Column<string>(type: "text", nullable: true),
                    BrewTime = table.Column<int>(type: "integer", nullable: false),
                    Remark = table.Column<string>(type: "text", nullable: true),
                    Aroma = table.Column<int>(type: "integer", nullable: true),
                    Acidity = table.Column<int>(type: "integer", nullable: true),
                    Body = table.Column<int>(type: "integer", nullable: true),
                    Sweetness = table.Column<int>(type: "integer", nullable: true),
                    Bitterness = table.Column<int>(type: "integer", nullable: true),
                    Aftertaste = table.Column<int>(type: "integer", nullable: true),
                    Extraction = table.Column<int>(type: "integer", nullable: true),
                    Overall = table.Column<int>(type: "integer", nullable: false),
                    CoffeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrewMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experiments_BrewMethods_BrewMethodId",
                        column: x => x.BrewMethodId,
                        principalTable: "BrewMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ExperimentParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExperimentId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrewParameterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperimentParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperimentParameters_BrewParameters_BrewParameterId",
                        column: x => x.BrewParameterId,
                        principalTable: "BrewParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExperimentParameters_Experiments_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "Experiments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrewParameters_BrewMethodId",
                table: "BrewParameters",
                column: "BrewMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Coffees_UserId",
                table: "Coffees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperimentParameters_BrewParameterId",
                table: "ExperimentParameters",
                column: "BrewParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperimentParameters_ExperimentId",
                table: "ExperimentParameters",
                column: "ExperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_BrewMethodId",
                table: "Experiments",
                column: "BrewMethodId");

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
                name: "ExperimentParameters");

            migrationBuilder.DropTable(
                name: "BrewParameters");

            migrationBuilder.DropTable(
                name: "Experiments");

            migrationBuilder.DropTable(
                name: "BrewMethods");

            migrationBuilder.DropTable(
                name: "Coffees");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
