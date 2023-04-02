using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobBoards.Data.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedJobApplicationsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplication_JobPosts_JobPostId",
                table: "JobApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplication_JobSeekers_JobSeekerId",
                table: "JobApplication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobApplication",
                table: "JobApplication");

            migrationBuilder.DropIndex(
                name: "IX_JobApplication_JobPostId",
                table: "JobApplication");

            migrationBuilder.RenameTable(
                name: "JobApplication",
                newName: "JobApplications");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplication_JobSeekerId",
                table: "JobApplications",
                newName: "IX_JobApplications_JobSeekerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobApplications",
                table: "JobApplications",
                columns: new[] { "JobPostId", "JobSeekerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_JobPosts_JobPostId",
                table: "JobApplications",
                column: "JobPostId",
                principalTable: "JobPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_JobSeekers_JobSeekerId",
                table: "JobApplications",
                column: "JobSeekerId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_JobPosts_JobPostId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_JobSeekers_JobSeekerId",
                table: "JobApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobApplications",
                table: "JobApplications");

            migrationBuilder.RenameTable(
                name: "JobApplications",
                newName: "JobApplication");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_JobSeekerId",
                table: "JobApplication",
                newName: "IX_JobApplication_JobSeekerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobApplication",
                table: "JobApplication",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_JobPostId",
                table: "JobApplication",
                column: "JobPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplication_JobPosts_JobPostId",
                table: "JobApplication",
                column: "JobPostId",
                principalTable: "JobPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplication_JobSeekers_JobSeekerId",
                table: "JobApplication",
                column: "JobSeekerId",
                principalTable: "JobSeekers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
