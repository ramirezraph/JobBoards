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
    public DbSet<JobApplication> JobApplications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(au => au.JobSeeker)
            .WithOne(js => js.User)
            .HasForeignKey<JobSeeker>(js => js.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

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

        modelBuilder.Entity<JobApplication>()
            .HasKey(ja => new { ja.JobPostId, ja.JobSeekerId });

        modelBuilder.Entity<JobApplication>()
            .HasOne(ja => ja.JobPost)
            .WithMany(jp => jp.JobApplications)
            .HasForeignKey(ja => ja.JobPostId);

        modelBuilder.Entity<JobApplication>()
            .HasOne(ja => ja.JobSeeker)
            .WithMany(js => js.JobApplications)
            .HasForeignKey(ja => ja.JobSeekerId);
    }
}