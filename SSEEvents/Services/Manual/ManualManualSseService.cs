using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.Services.Manual;

public class ManualManualSseService(ILogger<ManualManualSseService> logger) : IManualSseService
{
    // Thread-safe dictionary to store connected clients.
    private readonly ConcurrentDictionary<string, Client> _clients = new();

    public void AddClient(HttpContext context, ISet<string> topics)
    {
        var connectionId = context.Connection.Id;

        var client = new Client(context.Response, topics);
        if (_clients.TryAdd(connectionId, client))
        {
            context.Response.Headers["Content-Type"] = "text/event-streams";
            context.Response.Headers["Cache-Control"] = "no-cache";
            context.Response.Headers["Connection"] = "keep-alive";

            logger.LogInformation("SSE client connected: {ConnectionId}, Topics: {Topics}",
                connectionId, string.Join(", ", topics));
        }
    }

    public void RemoveClient(HttpContext context)
    {
        if (_clients.TryRemove(context.Connection.Id, out var client))
        {
            logger.LogInformation("SSE client disconnected: {ConnectionId}",
                context.Connection.Id);
        }
    }

    public async Task SendEventAsync(IServerSentEvent sse)
    {
        try
        {
            var message = sse.ToSseMessage();
            var data = Encoding.UTF8.GetBytes(message);
            var sentTasks = new List<Task>();

            foreach (var (connectionId, client) in _clients)
            {
                if (IsTopicMatch(sse.Topic, client.Topics))
                {
                    sentTasks.Add(WriteToClientAsync(connectionId, client, data, sse.Topic));
                }
            }

            await Task.WhenAll(sentTasks).ConfigureAwait(false);
        }
        catch (JsonException jsonEx)
        {
            logger.LogError(jsonEx, "Failed to serialize SSE event data for topic {Topic}", sse.Topic);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in SendEventAsync for topic {Topic}", sse.Topic);
        }
    }

    private static bool IsTopicMatch(string eventTopic, ISet<string> clientTopics)
    {
        foreach (var clientTopic in clientTopics)
        {
            if (string.Equals(clientTopic, eventTopic, StringComparison.OrdinalIgnoreCase))
            {
                // Exact match
                return true;
            }
            
            // we could implement a wildcard match here like
        }

        return false;
    }

    private async Task WriteToClientAsync(string connectionId, Client client, byte[] data, string topic)
    {
        try
        {
            await client.Response.Body.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            await client.Response.Body.FlushAsync().ConfigureAwait(false);
        }
        catch (IOException)
        {
            // Gracefully handle client disconnects
            if (_clients.TryRemove(connectionId, out _))
            {
                logger.LogInformation("SSE client {ConnectionId} disconnected during write for topic {Topic}. Removing from pool.", connectionId, topic);
            }
        }
        catch (OperationCanceledException)
        {
            // Gracefully handle client disconnectsOka
            if (_clients.TryRemove(connectionId, out _))
            {
                logger.LogInformation("SSE client {ConnectionId} cancelled connection during write for topic {Topic}. Removing from pool.", connectionId,
                    topic);
            }
        }
    }

    // Represents a connected client with its subscriptions.
    private record Client(HttpResponse Response, ISet<string> Topics);
}