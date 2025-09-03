using Lib.AspNetCore.ServerSentEvents;
using WebApplication1.Services;
using WebApplication1.Services.Library;
using WebApplication1.Services.Manual;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register the SSE service as a singleton to maintain client list across the application
builder.Services.AddSingleton<IManualSseService, ManualManualSseService>();

// Register the simple publisher interface for easy event publishing from business logic
builder.Services.AddScoped<ILibraryEventService, LibraryLibraryEventService>();
builder.Services.AddScoped<ModuleService>();

builder.Services.AddServerSentEvents();
builder.Services.AddControllers();
builder.Services.AddHostedService<EventProducerService>();
builder.Services.AddHostedService<SseClientGroupManager>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapServerSentEvents("/lib/events/stream");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();