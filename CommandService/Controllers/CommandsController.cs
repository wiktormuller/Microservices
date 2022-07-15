using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/c/platforms/{platformId}/[controller]")]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepository _commandRepository;
    
    public CommandsController(ICommandRepository commandRepository)
    {
        _commandRepository = commandRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandForPlatform(int platformId)
    {
        if (_commandRepository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var commands = _commandRepository.GetCommandsForPlatform(platformId);

        var result = commands.Select(x => new CommandReadDto
        {
            Id = x.Id,
            HowTo = x.CommandLine,
            CommandLine = x.CommandLine,
            PlatformId = x.PlatformId
        });
        
        return Ok(result);
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        if (_commandRepository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var command = _commandRepository.GetCommandById(platformId, commandId);

        if (command is null)
        {
            return NotFound();
        }

        var result = new CommandReadDto()
        {
            Id = command.Id,
            CommandLine = command.CommandLine,
            HowTo = command.HowTo,
            PlatformId = command.PlatformId
        };

        return Ok(result);
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
    {
        if (_commandRepository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var command = new Command
        {
            PlatformId = platformId,
            CommandLine = commandCreateDto.CommandLine,
            HowTo = commandCreateDto.HowTo
        };

        _commandRepository.CreateCommand(platformId, command);
        _commandRepository.SaveChanges();

        var commandReadDto = new CommandReadDto
        {
            Id = command.Id,
            HowTo = command.HowTo,
            CommandLine = command.CommandLine,
            PlatformId = command.PlatformId
        };

        return CreatedAtRoute(nameof(GetCommandForPlatform),
            new {PlatformId = platformId, CommandId = commandReadDto.Id}, commandReadDto);
    }
}