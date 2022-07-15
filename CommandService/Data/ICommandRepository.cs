﻿using CommandService.Models;

namespace CommandService.Data;

public interface ICommandRepository
{
    bool SaveChanges();
    
    // Platforms
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExists(int platformId);
    
    // Commands
    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommandById(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}