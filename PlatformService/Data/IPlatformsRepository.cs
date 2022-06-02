using PlatformService.Models;

namespace PlatformService.Data;

public interface IPlatformsRepository
{
    bool SaveChanges();
    IEnumerable<Platform> GetAll();
    Platform GetById(int id);
    void Create(Platform platform);
}