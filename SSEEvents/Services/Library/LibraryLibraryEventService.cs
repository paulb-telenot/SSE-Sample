using System.Text.Json;
using Lib.AspNetCore.ServerSentEvents;

namespace WebApplication1.Services.Library;

public class LibraryLibraryEventService(IServerSentEventsService sseService) : ILibraryEventService
{
    public Task SendEventAsync(string topic, object data)
    {
        var message = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return sseService.SendEventAsync(topic, message);
    }
}