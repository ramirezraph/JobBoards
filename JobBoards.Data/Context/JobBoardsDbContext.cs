using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Context;

public class JobBoardsDbContext : DbContext
{
    public JobBoardsDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}