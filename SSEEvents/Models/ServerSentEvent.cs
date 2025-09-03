using System.Text.Json;

namespace WebApplication1.Models;

/// <summary>
///     Non-generic interface for common event properties.
/// </summary>
public interface IServerSentEvent
{
    string Topic { get; }
    string ToSseMessage();
}

/// <summary>
///     Represents a single, strongly-typed event to be sent to a client.
/// </summary>
/// <typeparam name="T">The type of the data payload.</typeparam>
public class ServerSentEvent<T> : IServerSentEvent
{
    /// <summary>
    ///     The name of the event (e.g., "order-updated", "item-deleted").
    /// </summary>
    public string Event { get; init; } = "message";

    /// <summary>
    ///     The strongly-typed data payload for the event.
    /// </summary>
    public T Data { get; init; }

    /// <summary>
    ///     The specific topic this event relates to (e.g., "orders:123", "invoices:*").
    /// </summary>
    public required string Topic { get; init; }

    /// <summary>
    ///     Formats the event into the official SSE message format.
    /// </summary>
    public string ToSseMessage()
    {
        var jsonData = JsonSerializer.Serialize(Data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return $"event: {Event}\n" +
               $"data: {jsonData}\n\n";
    }
}

/// <summary>
///     Non-generic representation of a Server-Sent Event for flexible data types.
/// </summary>
public class ServerSentEvent : IServerSentEvent
{
    public string Event { get; init; } = "message";
    public object? Data { get; init; } // Made nullable to address warning
    public required string Topic { get; init; }

    public string ToSseMessage()
    {
        var jsonData = Data != null
            ? JsonSerializer.Serialize(Data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
            : "";

        return $"event: {Event}\n" +
               $"data: {jsonData}\n\n";
    }
}