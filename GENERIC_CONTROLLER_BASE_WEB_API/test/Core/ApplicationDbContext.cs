using Microsoft.EntityFrameworkCore;
using test.Entities;
using test.Seed;

namespace test.Core
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<ApnName> ApnNames { get; set; }
        public DbSet<ApnPassword> ApnPasswords { get; set; }
        public DbSet<ApnAddress> ApnAddresses { get; set; }
        public DbSet<Station> Stations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data from DataSeeder class
            DataSeeder.SeedData(modelBuilder);
        }
    }
} 