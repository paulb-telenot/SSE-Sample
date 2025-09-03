using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("library-events")]
public class LibrarySseController : ControllerBase
{
    [HttpGet("stream")]
    public async Task Get()
    {
        // The library handles the connection and client management.
        // We just need to ensure the connection is kept open.
        // The library's middleware will manage the SSE stream.
        await Task.Delay(Timeout.Infinite, HttpContext.RequestAborted).ConfigureAwait(false);
    }
}