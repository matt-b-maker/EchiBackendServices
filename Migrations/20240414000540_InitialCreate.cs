using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchiBackendServices.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncludeRadonAddendum = table.Column<bool>(type: "bit", nullable: false),
                    DoNotIncludeRadonAddendum = table.Column<bool>(type: "bit", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainInspectionImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainInspectionImageFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerializedInspection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerializedClient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientAddressLineOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientAddressLineTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientAddressCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientAddressState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientAddressZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RadonFee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionAddressLineOne = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionAddressLineTwo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionAddressCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionAddressState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionAddressZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgencyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionDateString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresentAtInspection = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
