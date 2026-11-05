using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deporcheves.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWeightedAverageCostAndPayables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Purchases",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Purchases");
        }
    }
}
