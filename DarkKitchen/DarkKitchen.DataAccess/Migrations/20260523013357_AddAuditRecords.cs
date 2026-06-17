using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkKitchen.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    ChangeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponsibleUser = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRecords", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 0,
                column: "Permissions",
                value: "[3,4,5,6,7,10,11,12,13,14,15,16,17,18,19,20,21,22,23]");

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 1,
                column: "Permissions",
                value: "[0,1,15,20]");

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 2,
                column: "Permissions",
                value: "[2,3,4,6,7,8,9]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditRecords");

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
