using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace DefaultNamespace;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformsRepository _platformsRepository;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(IPlatformsRepository platformsRepository, 
        ICommandDataClient commandDataClient, 
        IMessageBusClient messageBusClient)
    {
        _platformsRepository = platformsRepository;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetAll()
    {
        var platforms = _platformsRepository.GetAll();
        var result = platforms.Select(platform => new PlatformReadDto
        {
            Id = platform.Id,
            Name = platform.Name,
            Cost = platform.Cost,
            Publisher = platform.Publisher
        });

        return Ok(result);
    }

    [HttpGet("{id}", Name = "GetById")]
    public ActionResult<PlatformReadDto> GetById(int id)
    {
        var platform = _platformsRepository.GetById(id);
        if (platform is null)
        {
            return NotFound();
        }

        var result = new PlatformReadDto()
        {
            Id = platform.Id,
            Name = platform.Name,
            Cost = platform.Cost,
            Publisher = platform.Publisher
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> Create(PlatformCreateDto request)
    {
        var platform = new Platform
        {
            Name = request.Name,
            Cost = request.Cost,
            Publisher = request.Publisher
        };

        _platformsRepository.Create(platform);
        _platformsRepository.SaveChanges();

        var result = new PlatformReadDto
        {
            Id = platform.Id,
            Name = platform.Name,
            Cost = platform.Cost,
            Publisher = platform.Publisher
        };

        // Send Sync Message
        try
        {
            await _commandDataClient.SendPlatformToCommand(result);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send synchronously: {e.Message}");
        }
        
        // Send Async Message
        try
        {
            var platformPublishedDto = new PlatformPublishedDto
            {
                Id = result.Id,
                Name = result.Name,
                Event = "Platform_Published"
            };
            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send asynchronously: {e.Message}.");
        }

        return CreatedAtRoute(nameof(GetById), new { Id = result.Id }, result);
    }
}