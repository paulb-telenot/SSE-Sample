using WebApplication1.Services.Library;

namespace WebApplication1.Services;

/// <summary>
///     Example service demonstrating the new, simplified way to publish an SSE event.
/// </summary>
public class ModuleService(ILibraryEventService libraryEventService)
{
    public async Task SendEvent(string topic, object data)
    {
        await libraryEventService.SendEventAsync(topic, data).ConfigureAwait(false);
    }
}