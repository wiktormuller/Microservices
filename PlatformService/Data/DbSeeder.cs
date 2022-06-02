using PlatformService.Models;

namespace PlatformService.Data;

public static class DbSeeder
{
    public static void PrepareSeeder(IApplicationBuilder appBuilder)
    {
        using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
        {
            var appDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
            SeedData(appDbContext);
        }
    }

    private static void SeedData(AppDbContext dbContext)
    {
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