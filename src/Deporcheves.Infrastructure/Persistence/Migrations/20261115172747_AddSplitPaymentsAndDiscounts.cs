using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deporcheves.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSplitPaymentsAndDiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountCard",
                table: "Sales",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountCash",
                table: "Sales",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTransfer",
                table: "Sales",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountCard",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "AmountCash",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "AmountTransfer",
                table: "Sales");
        }
    }
}
