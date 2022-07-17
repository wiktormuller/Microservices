using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc;

public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformsRepository _platformsRepository;
    
    public GrpcPlatformService(IPlatformsRepository platformsRepository)
    {
        _platformsRepository = platformsRepository;
    }

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
    {
        var response = new PlatformResponse();

        var platforms = _platformsRepository.GetAll();

        foreach (var platform in platforms)
        {
            response.Platform.Add(new GrpcPlatformModel
            {
                PlatformId = platform.Id,
                Name = platform.Name,
                Publisher = platform.Publisher
            });
        }

        return Task.FromResult(response);
    }
}