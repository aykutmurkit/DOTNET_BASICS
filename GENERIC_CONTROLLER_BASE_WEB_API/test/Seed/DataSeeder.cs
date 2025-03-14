using Microsoft.EntityFrameworkCore;
using test.Core;

namespace test.Seed
{
    public static class DataSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            SeedApnNames(modelBuilder);
            SeedApnPasswords(modelBuilder);
            SeedApnAddresses(modelBuilder);
            SeedStations(modelBuilder);
            SeedDevices(modelBuilder);
        }

        private static void SeedApnNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.ApnName>().HasData(
                new Entities.ApnName { Id = 1, Value = "APN1", CreatedDate = DateTime.UtcNow },
                new Entities.ApnName { Id = 2, Value = "APN2", CreatedDate = DateTime.UtcNow },
                new Entities.ApnName { Id = 3, Value = "APN3", CreatedDate = DateTime.UtcNow }
            );
        }

        private static void SeedApnPasswords(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.ApnPassword>().HasData(
                new Entities.ApnPassword { Id = 1, Value = "Pass1", CreatedDate = DateTime.UtcNow },
                new Entities.ApnPassword { Id = 2, Value = "Pass2", CreatedDate = DateTime.UtcNow },
                new Entities.ApnPassword { Id = 3, Value = "Pass3", CreatedDate = DateTime.UtcNow }
            );
        }

        private static void SeedApnAddresses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.ApnAddress>().HasData(
                new Entities.ApnAddress { Id = 1, Value = "Address1", CreatedDate = DateTime.UtcNow },
                new Entities.ApnAddress { Id = 2, Value = "Address2", CreatedDate = DateTime.UtcNow },
                new Entities.ApnAddress { Id = 3, Value = "Address3", CreatedDate = DateTime.UtcNow }
            );
        }

        private static void SeedStations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Station>().HasData(
                new Entities.Station { Id = 1, Name = "Station1", Location = "Location1", Capacity = 100, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Entities.Station { Id = 2, Name = "Station2", Location = "Location2", Capacity = 200, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Entities.Station { Id = 3, Name = "Station3", Location = "Location3", Capacity = 300, IsActive = false, CreatedDate = DateTime.UtcNow }
            );
        }

        private static void SeedDevices(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Device>().HasData(
                new Entities.Device { 
                    Id = 1, 
                    Name = "Device1", 
                    SerialNumber = "SN001",
                    ApnNameId = 1,
                    ApnPasswordId = 1,
                    ApnAddressId = 1,
                    CreatedDate = DateTime.UtcNow
                },
                new Entities.Device { 
                    Id = 2, 
                    Name = "Device2", 
                    SerialNumber = "SN002",
                    ApnNameId = 2,
                    ApnPasswordId = 2,
                    ApnAddressId = 2,
                    CreatedDate = DateTime.UtcNow
                },
                new Entities.Device { 
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