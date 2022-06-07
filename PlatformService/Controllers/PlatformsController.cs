using Microsoft.AspNetCore.Mvc;
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
    
    public PlatformsController(IPlatformsRepository platformsRepository, ICommandDataClient commandDataClient)
    {
        _platformsRepository = platformsRepository;
        _commandDataClient = commandDataClient;
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

        try
        {
            await _commandDataClient.SendPlatformToCommand(result);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send synchronously: {e.Message}");
        }

        return CreatedAtRoute(nameof(GetById), new { Id = result.Id }, result);
    }
}