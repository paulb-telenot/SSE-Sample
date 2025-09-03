using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Manual;

namespace WebApplication1.Controllers;

[ApiController]
[Route("events")]
public class SseController(IManualSseService manualSseService) : ControllerBase
{
    [HttpGet("stream")]
    public async Task GetEventStream([FromQuery] string? topics)
    {
        // Parse topic subscriptions from the query string (e.g., "?topics=orders:*,invoices:12345")
        var topicSet = new HashSet<string>(topics?.Split(',') ?? Array.Empty<string>(),
            StringComparer.OrdinalIgnoreCase);

        manualSseService.AddClient(HttpContext, topicSet);

        try
        {
            // Keep the connection alive by waiting until the client disconnects.
            // A keep-alive comment is sent every 15 seconds to prevent timeouts.
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                await HttpContext.Response.WriteAsync(": keep-alive\n\n").ConfigureAwait(false);
                await HttpContext.Response.Body.FlushAsync().ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(15), HttpContext.RequestAborted).ConfigureAwait(false);
            }
        }
        catch (TaskCanceledException)
        {
            // This is expected when the client disconnects.
        }
        finally
        {
            // Ensure the client is removed from the service when the connection is closed.
            manualSseService.RemoveClient(HttpContext);
        }
    }
}