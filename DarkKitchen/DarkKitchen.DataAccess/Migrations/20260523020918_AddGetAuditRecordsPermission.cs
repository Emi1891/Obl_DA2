using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkKitchen.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddGetAuditRecordsPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 0,
                column: "Permissions",
                value: "[3,4,5,6,7,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Role",
                keyValue: 0,
                column: "Permissions",
                value: "[3,4,5,6,7,10,11,12,13,14,15,16,17,18,19,20,21,22,23]");
        }
    }
}
