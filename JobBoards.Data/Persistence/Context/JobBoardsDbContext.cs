using JobBoards.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Persistence.Context;

public class JobBoardsDbContext : IdentityDbContext<ApplicationUser>
{
    public JobBoardsDbContext(DbContextOptions<JobBoardsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}