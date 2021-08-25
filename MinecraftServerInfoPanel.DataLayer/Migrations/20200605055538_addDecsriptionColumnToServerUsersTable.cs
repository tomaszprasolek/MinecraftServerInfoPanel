using Microsoft.EntityFrameworkCore.Migrations;

namespace MinecraftServerInfoPanel.Migrations
{
    public partial class addDecsriptionColumnToServerUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ServerUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ServerUsers");
        }
    }
}
