namespace WebApplication1.Services.Library;

public interface ILibraryEventService
{
    Task SendEventAsync(string topic, object data);
}