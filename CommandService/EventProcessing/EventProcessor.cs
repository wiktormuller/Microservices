using System.Text.Json;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventProcessor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        if (eventType == EventType.PlatformPublished)
        {
            AddPlatform(message);
        }
        else
        {
            return;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

            var publishedPlatformDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = new Platform
                {
                    Name = publishedPlatformDto.Name,
                    ExternalId = publishedPlatformDto.Id
                };

                if (!repo.ExternalPlatformExist(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChanges();
                    Console.WriteLine("--> Platform added.");
                }
                else
                {
                    Console.WriteLine("--> Platform already exists...");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"--> Could not add Platform to DB {exception.Message}.");
            }
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event ");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        if (eventType.Event == "Platform_Published")
        {
            Console.WriteLine("--> Platform Published event detected.");
            return EventType.PlatformPublished;
        }
        else
        {
            Console.WriteLine("--> Could not determine the event type.");
            return EventType.Undetermined;
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}