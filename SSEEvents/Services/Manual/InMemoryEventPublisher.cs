using WebApplication1.Models;

namespace WebApplication1.Services.Manual;

/// <summary>
///     In-memory implementation of the event publisher.
/// </summary>
public class InMemoryEventPublisher(IManualSseService manualSseService) : IEventPublisher
{
    public Task PublishAsync<T>(string tenantId, string topic, string eventName, T data)
    {
        var sse = new ServerSentEvent<T>
        {
            Topic = topic,
            Event = eventName,
            Data = data
        };

        return manualSseService.SendEventAsync(sse);
    }
}