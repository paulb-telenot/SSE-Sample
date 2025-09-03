using WebApplication1.Models;
using WebApplication1.Services.Library;
using WebApplication1.Services.Manual;

namespace WebApplication1.Services;

public class EventProducerService(IServiceScopeFactory scopeFactory, ILogger<EventProducerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("EventProducerService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var libraryEventService = scope.ServiceProvider.GetRequiredService<ILibraryEventService>();
                var manualSseService = scope.ServiceProvider.GetRequiredService<IManualSseService>();
                var topic = "sharedTopic";

                // Send event via manual SSE implementation
                var manualEvent = new ServerSentEvent
                {
                    Data = new { Message = $"SHARED Manual update from producer at {DateTime.UtcNow}" },
                    Topic = topic
                };                
                var manualWrongEvent = new ServerSentEvent
                {
                    Data = new { Message = $"SHARED Manual update from producer at {DateTime.UtcNow}" },
                    Topic = "DEFAULT"
                };
                await manualSseService.SendEventAsync(manualEvent).ConfigureAwait(false);
                await manualSseService.SendEventAsync(manualWrongEvent).ConfigureAwait(false);
                logger.LogInformation("Sent manual SSE event.");

                // Send event via library SSE implementation
                var libraryEventData = new { Message = $"SHARED Library update from producer at {DateTime.UtcNow}" };
                var libraryWrongEventData = new { Message = $"WRONG Library update from producer at {DateTime.UtcNow}" };
                await libraryEventService.SendEventAsync(topic, libraryEventData).ConfigureAwait(false);
                await libraryEventService.SendEventAsync("DEFAULT", libraryWrongEventData).ConfigureAwait(false);
                logger.LogInformation("Sent library SSE event.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending event from producer.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ConfigureAwait(false);
        }

        logger.LogInformation("EventProducerService stopped.");
    }
}