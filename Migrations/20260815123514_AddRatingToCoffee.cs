using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingToCoffee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Coffees",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Coffees");
        }
    }
}
