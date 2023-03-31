using JobBoards.Data.Entities;
using JobBoards.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Persistence.Context;

public class JobBoardsDbContext : IdentityDbContext<ApplicationUser>
{
    public JobBoardsDbContext(DbContextOptions<JobBoardsDbContext> options) : base(options)
    {
    }

    public DbSet<JobType> JobTypes { get; set; } = null!;
    public DbSet<JobCategory> JobCategories { get; set; } = null!;
    public DbSet<JobLocation> JobLocations { get; set; } = null!;
    public DbSet<Resume> Resumes { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}