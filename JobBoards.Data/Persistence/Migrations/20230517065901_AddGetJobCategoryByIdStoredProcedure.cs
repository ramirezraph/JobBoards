using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobBoards.Data.Persistence.Migrations
{
    public partial class AddGetJobCategoryByIdStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE OR ALTER PROCEDURE getJobCategoryById(@id AS UNIQUEIDENTIFIER)
            AS
            BEGIN
                SELECT * FROM JobCategories WHERE Id = @id;
            END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sp = @"DROP PROCEDURE getJobCategoryById";

            migrationBuilder.Sql(sp);
        }
    }
}
