using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEventsAPI.Services.GraphSync;

namespace StudentEventsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SyncController : ControllerBase
{
    private readonly IGraphSyncService _syncService;

    public SyncController(IGraphSyncService syncService)
    {
        _syncService = syncService;
    }

    [HttpPost("students")]
    public async Task<IActionResult> SyncStudents(CancellationToken cancellationToken)
    {
        await _syncService.SyncStudentsAsync(cancellationToken);
        return Ok(new { Message = "Sincronização de estudantes iniciada com sucesso" });
    }

    [HttpPost("events")]
    public async Task<IActionResult> SyncEvents(CancellationToken cancellationToken)
    {
        await _syncService.SyncEventsAsync(cancellationToken);
        return Ok(new { Message = "Sincronização de eventos iniciada com sucesso" });
    }
}
