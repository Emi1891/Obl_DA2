using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkKitchen.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TranslateShippingTypeNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ShippingTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Same day");

            migrationBuilder.UpdateData(
                table: "ShippingTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Next day");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ShippingTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "En el día");

            migrationBuilder.UpdateData(
                table: "ShippingTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Día siguiente");
        }
    }
}
