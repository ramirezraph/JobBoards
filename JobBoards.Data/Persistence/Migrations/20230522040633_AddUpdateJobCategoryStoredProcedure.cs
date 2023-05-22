using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobBoards.Data.Persistence.Migrations
{
    public partial class AddUpdateJobCategoryStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE OR ALTER PROCEDURE updateJobCategory
                @Id UNIQUEIDENTIFIER, 
                @Name NVARCHAR(MAX), 
                @Description NVARCHAR(MAX) = NULL,
                @UpdatedAt DATETIME2
            AS
            BEGIN
                UPDATE 
                    JobCategories
                SET 
                    Name = @Name,
                    Description = @Description,
                    UpdatedAt = @UpdatedAt
                WHERE 
                    Id = @Id;
            END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sp = @"DROP PROCEDURE updateJobCategory";

            migrationBuilder.Sql(sp);
        }
    }
}
