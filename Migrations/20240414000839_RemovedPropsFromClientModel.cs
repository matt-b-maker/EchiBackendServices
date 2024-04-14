using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchiBackendServices.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPropsFromClientModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerializedClient",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "SerializedInspection",
                table: "Clients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerializedClient",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerializedInspection",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
