using Microsoft.EntityFrameworkCore;

namespace PowerUsageCUI.DataModel.PowerModel
{
    public class PowerModelContext : DbContext
    {
        public DbSet<ThreePhaseMeter> ThreePhaseMeterSet { get; set; }
        private string DbPath { get; init; }

        public PowerModelContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "PowerMeter.db");
            //Console.WriteLine($"Database path: {DbPath}.");
        }

        // Create a Sql lite database.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        { 
            options.UseSqlite($"Data Source={DbPath}");
            //options.LogTo(Console.Write);
            //options.EnableSensitiveDataLogging();
        }
    }
}
