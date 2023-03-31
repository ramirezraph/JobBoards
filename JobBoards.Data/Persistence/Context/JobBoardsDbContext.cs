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
    public DbSet<JobPost> JobPosts { get; set; } = null!;
    public DbSet<JobSeeker> JobSeekers { get; set; } = null!;
    public DbSet<Resume> Resumes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JobSeeker>()
            .HasOne(js => js.Resume)
            .WithOne(r => r.JobSeeker)
            .HasForeignKey<Resume>(r => r.JobSeekerId)
            .IsRequired(false);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.JobPosts)
            .WithOne(jp => jp.CreatedBy)
            .HasForeignKey(jp => jp.CreatedById)
            .OnDelete(DeleteBehavior.Cascade);
    }
}