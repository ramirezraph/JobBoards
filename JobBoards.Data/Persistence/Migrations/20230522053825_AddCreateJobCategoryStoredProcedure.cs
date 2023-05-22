using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobBoards.Data.Persistence.Migrations
{
    public partial class AddCreateJobCategoryStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE OR ALTER PROCEDURE createJobCategory
                @Name NVARCHAR(MAX), 
                @Description NVARCHAR(MAX) = NULL,
                @CreatedAt DATETIME2
            AS
            BEGIN
                INSERT INTO JobCategories (Id, Name, Description, CreatedAt, UpdatedAt, DeletedAt)
                VALUES (NEWID(), @Name, @Description, @CreatedAt, @CreatedAt, NULL)
            END";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sp = @"DROP PROCEDURE createJobCategory";

            migrationBuilder.Sql(sp);
        }
    }
}
