using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformsRepository : IPlatformsRepository
{
    private readonly AppDbContext _appDbContext;
    
    public PlatformsRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    public bool SaveChanges()
    {
        return _appDbContext.SaveChanges() > 0;
    }

    public IEnumerable<Platform> GetAll()
    {
        return _appDbContext.Platforms.ToList();
    }

    public Platform GetById(int id)
    {
        return _appDbContext.Platforms.FirstOrDefault(platform => platform.Id == id);
    }

    public void Create(Platform platform)
    {
        if (platform is null)
        {
            throw new ArgumentNullException(nameof(platform));
        }

        _appDbContext.Platforms.Add(platform);
    }
}