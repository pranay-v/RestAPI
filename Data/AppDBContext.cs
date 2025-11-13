using Microsoft.EntityFrameworkCore;
using MergeApi.Models;

namespace MergeApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<MergeRecord> Merges => Set<MergeRecord>();
}
