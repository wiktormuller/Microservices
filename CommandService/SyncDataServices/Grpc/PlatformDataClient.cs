using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration _configuration;
    
    public PlatformDataClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        Console.WriteLine($"--> Calling gRPC Service {_configuration["GrpcPlatform"]}.");

        var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllPlatforms(request);

            return reply.Platform.Select(x => new Platform
            {
                ExternalId = x.PlatformId,
                Name = x.Name
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Couldn't not call gRPC Server {e.Message}.");
            return null;
        }
    }
}