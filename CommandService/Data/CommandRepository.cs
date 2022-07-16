using CommandService.Models;

namespace CommandService.Data;

public class CommandRepository : ICommandRepository
{
    private readonly AppDbContext _dbContext;
    
    public CommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public bool SaveChanges()
    {
        return _dbContext.SaveChanges() >= 0;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _dbContext.Platforms.ToList();
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform is null)
        {
            throw new ArgumentNullException(nameof(platform));
        }

        _dbContext.Platforms.Add(platform);
    }

    public bool PlatformExists(int platformId)
    {
        return _dbContext.Platforms.Any(p => p.Id == platformId);
    }

    public bool ExternalPlatformExist(int externalPlatformId)
    {
        return _dbContext.Platforms.Any(p => p.ExternalId == externalPlatformId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _dbContext.Commands.Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform.Name);
    }

    public Command GetCommandById(int platformId, int commandId)
    {
        return _dbContext.Commands.FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        command.PlatformId = platformId;
        _dbContext.Commands.Add(command);
    }
}