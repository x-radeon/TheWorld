using System;
using Microsoft.Data.Entity;

namespace TheWorld.Models
{
    public class WorldContext : DbContext
    {
        public WorldContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Stop> Stops { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            var connectionString = Startup.Configuration["Data:WorldContextConnectionLocal"];
#else
            var connectionString = Startup.Configuration["Data:WorldContextConnection"];
#endif
            optionsBuilder.UseSqlServer(connectionString);

            base.OnConfiguring(optionsBuilder);
        }
    }
}