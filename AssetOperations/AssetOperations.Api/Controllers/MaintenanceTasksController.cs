using AssetOperations.Application.Abstractions;
using AssetOperations.Application.Dtos;
using AssetOperations.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace AssetOperations.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MaintenanceTasksController : ControllerBase
{
    private readonly IMaintenanceTaskRepository _repo;
    private readonly GetOverdueMaintenancesUseCase _getOverdue;

    public MaintenanceTasksController(
        IMaintenanceTaskRepository repo,
        GetOverdueMaintenancesUseCase getOverdue)
    {
        _repo = repo;
        _getOverdue = getOverdue;
    }

    // GET /api/MaintenanceTasks/overdue?currentRunningHour=12345
    [HttpGet("overdue")]
    public async Task<ActionResult<IReadOnlyList<MaintenanceListItemDto>>> GetOverdue(
        [FromQuery] int currentRunningHour,
        CancellationToken ct)
    {
        var list = await _repo.ListAsync(ct);

        var result = _getOverdue.Execute(
            list,
            DateTime.UtcNow,
            currentRunningHour);

        return Ok(result);
    }
}