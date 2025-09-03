using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Services
{
    public class SseClientGroupManager(IServerSentEventsService serverSentEventsService) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            serverSentEventsService.ClientConnected += OnClientConnected;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            serverSentEventsService.ClientConnected -= OnClientConnected;
            return Task.CompletedTask;
        }

        private async void OnClientConnected(object sender, ServerSentEventsClientConnectedArgs e)
        {
            string topic = e.Request.Query["topic"].ToString();
            if (string.IsNullOrEmpty(topic))
            {
                topic = "default"; // Default topic if none is provided
            }
            serverSentEventsService.AddToGroup(topic, e.Client);
        }
    }
}
