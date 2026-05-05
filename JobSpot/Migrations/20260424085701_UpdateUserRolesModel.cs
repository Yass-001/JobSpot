using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSpot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRolesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "AspNetUserRoles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "AspNetUserRoles");
        }
    }
}
