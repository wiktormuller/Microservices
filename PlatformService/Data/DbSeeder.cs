using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class DbSeeder
{
    public static void PrepareSeeder(IApplicationBuilder appBuilder, bool isProduction)
    {
        using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
        {
            var appDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
            SeedData(appDbContext, isProduction);
        }
    }

    private static void SeedData(AppDbContext dbContext, bool isProduction)
    {
        if (isProduction)
        {
            Console.WriteLine("--> Attempting to apply migrations...");
            try
            {
                dbContext.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not run migrations: {e.Message}.");
            }
        }
    
        if (!dbContext.Platforms.Any())
        {
            Console.WriteLine("--> Seeding data...");
            dbContext.Platforms.AddRange(
                new Platform
                {
                    Name = "Dot Net",
                    Publisher = "Microsoft",
                    Cost = "Free"
                },
                new Platform
                {
                    Name = "SQL Server Express",
                    Publisher = "Microsoft",
                    Cost = "Free"
                },
                new Platform
                {
                    Name = "Kubernetes",
                    Publisher = "Cloud Native Computing Foundation",
                    Cost = "Free"
                });

            dbContext.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> We already have data.");
        }
    }
}