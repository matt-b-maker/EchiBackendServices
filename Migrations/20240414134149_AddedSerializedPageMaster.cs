using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchiBackendServices.Migrations
{
    /// <inheritdoc />
    public partial class AddedSerializedPageMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerializedPageMaster",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerializedPageMaster",
                table: "Clients");
        }
    }
}
