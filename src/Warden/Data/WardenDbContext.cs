using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Warden.Data;

public class WardenDbContext : AbpDbContext<WardenDbContext>
{
    public WardenDbContext(DbContextOptions<WardenDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
