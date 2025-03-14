using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using test.Core;

namespace test.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Ensures the database is created and seeded
        /// </summary>
        public static IApplicationBuilder EnsureDatabaseCreated(this IApplicationBuilder app, bool recreateDatabase = false)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                if (recreateDatabase)
                {
                    // Drop existing database
                    context.Database.EnsureDeleted();
                }
                
                // Create new database with seed data
                context.Database.EnsureCreated();
            }

            return app;
        }
    }
} 