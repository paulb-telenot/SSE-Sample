using WebApplication1.Models;

namespace WebApplication1.Services.Manual;

public interface IManualSseService
{
    void AddClient(HttpContext context, ISet<string> topics);
    void RemoveClient(HttpContext context);
    Task SendEventAsync(IServerSentEvent sse);
}