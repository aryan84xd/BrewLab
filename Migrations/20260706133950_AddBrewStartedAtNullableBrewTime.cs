using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddBrewStartedAtNullableBrewTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BrewTime",
                table: "Experiments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "BrewStartedAt",
                table: "Experiments",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrewStartedAt",
                table: "Experiments");

            migrationBuilder.AlterColumn<int>(
                name: "BrewTime",
                table: "Experiments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
