using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DarkKitchen.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingTypes", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 0,
                column: "Permissions",
                value: "[3,4,5,6,7,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26]");

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 1,
                column: "Permissions",
                value: "[0,1,15,20,24]");

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 2,
                column: "Permissions",
                value: "[2,3,4,6,7,8,9]");

            migrationBuilder.InsertData(
                table: "ShippingTypes",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Express", 250m },
                    { 2, "En el día", 200m },
                    { 3, "Día siguiente", 180m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingTypes");

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 0,
                column: "Permissions",
                value: "[3,4,5,6,7,9,10,11,12,13,14,15,16,17,18,19,20,21,22]");

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 1,
                column: "Permissions",
                value: "[0,1,14,19]");

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 2,
                column: "Permissions",
                value: "[2,3,4,6,7,8]");
        }
    }
}
