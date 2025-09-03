namespace WebApplication1.Services.Manual;

/// <summary>
///     Defines a simple interface for publishing events to be sent to clients.
///     This abstracts away the underlying SSE implementation.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    ///     Publishes an event to a specific topic for a specific tenant.
    /// </summary>
    /// <typeparam name="T">The type of the event data.</typeparam>
    /// <param name="tenantId">The tenant to target.</param>
    /// <param name="topic">The topic to publish to (e.g., "orders:123").</param>
    /// <param name="eventName">The name of the event (e.g., "order-updated").</param>
    /// <param name="data">The event payload.</param>
    Task PublishAsync<T>(string tenantId, string topic, string eventName, T data);
}