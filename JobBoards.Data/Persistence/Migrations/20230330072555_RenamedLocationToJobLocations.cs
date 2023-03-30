using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobBoards.Data.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedLocationToJobLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.RenameTable(
                name: "Locations",
                newName: "JobLocations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobLocations",
                table: "JobLocations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JobLocations",
                table: "JobLocations");

            migrationBuilder.RenameTable(
                name: "JobLocations",
                newName: "Locations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Id");
        }
    }
}
