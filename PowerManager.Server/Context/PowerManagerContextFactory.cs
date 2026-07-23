using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PowerManager.Server.Context
{
    public class PowerManagerContextFactory : IDesignTimeDbContextFactory<PowerManagerContext>
    {
        public PowerManagerContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<PowerManagerContext> builder = new DbContextOptionsBuilder<PowerManagerContext>().UseSqlite($"Data Source={args[0]}");
            return new PowerManagerContext(builder.Options);
        }
    }
}
