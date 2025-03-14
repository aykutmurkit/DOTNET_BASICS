using Microsoft.EntityFrameworkCore;
using test.Entities;

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

            // Seed ApnNames
            modelBuilder.Entity<ApnName>().HasData(
                new ApnName { Id = 1, Name = "APN1", CreatedDate = DateTime.UtcNow },
                new ApnName { Id = 2, Name = "APN2", CreatedDate = DateTime.UtcNow },
                new ApnName { Id = 3, Name = "APN3", CreatedDate = DateTime.UtcNow }
            );

            // Seed ApnPasswords
            modelBuilder.Entity<ApnPassword>().HasData(
                new ApnPassword { Id = 1, Password = "Pass1", CreatedDate = DateTime.UtcNow },
                new ApnPassword { Id = 2, Password = "Pass2", CreatedDate = DateTime.UtcNow },
                new ApnPassword { Id = 3, Password = "Pass3", CreatedDate = DateTime.UtcNow }
            );

            // Seed ApnAddresses
            modelBuilder.Entity<ApnAddress>().HasData(
                new ApnAddress { Id = 1, Address = "Address1", CreatedDate = DateTime.UtcNow },
                new ApnAddress { Id = 2, Address = "Address2", CreatedDate = DateTime.UtcNow },
                new ApnAddress { Id = 3, Address = "Address3", CreatedDate = DateTime.UtcNow }
            );

            // Seed Stations
            modelBuilder.Entity<Station>().HasData(
                new Station { Id = 1, Name = "Station1", Location = "Location1", Capacity = 100, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Station { Id = 2, Name = "Station2", Location = "Location2", Capacity = 200, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Station { Id = 3, Name = "Station3", Location = "Location3", Capacity = 300, IsActive = false, CreatedDate = DateTime.UtcNow }
            );

            // Seed Devices
            modelBuilder.Entity<Device>().HasData(
                new Device { 
                    Id = 1, 
                    Name = "Device1", 
                    SerialNumber = "SN001",
                    ApnNameId = 1,
                    ApnPasswordId = 1,
                    ApnAddressId = 1,
                    CreatedDate = DateTime.UtcNow
                },
                new Device { 
                    Id = 2, 
                    Name = "Device2", 
                    SerialNumber = "SN002",
                    ApnNameId = 2,
                    ApnPasswordId = 2,
                    ApnAddressId = 2,
                    CreatedDate = DateTime.UtcNow
                },
                new Device { 
                    Id = 3, 
                    Name = "Device3", 
                    SerialNumber = "SN003",
                    ApnNameId = 3,
                    ApnPasswordId = 3,
                    ApnAddressId = 3,
                    CreatedDate = DateTime.UtcNow
                }
            );
        }
    }
} 