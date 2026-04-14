using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSpot.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToAspNetUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "AspNetUserRoles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
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
