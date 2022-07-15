using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepository _commandRepository;
    
    public PlatformsController(ICommandRepository commandRepository)
    {
        _commandRepository = commandRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
    {
        var platforms = _commandRepository.GetAllPlatforms();

        var result = platforms.Select(x => new PlatformReadDto
        {
            Id = x.Id,
            Name = x.Name
        });
        
        return Ok(result);
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POSt # Command Service");

        return Ok("Inbound text from Platforms Controller");
    }
}