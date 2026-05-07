using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EndpointGuardian.Api.Controllers;

[ApiController]
[Route("api/audit-events")]
public class AuditEventsController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditEventsController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    [Authorize(Policy = "CanReadAudit")]
    public async Task<ActionResult<PagedAuditEventsResponse>> GetAuditEvents(
        [FromQuery] GetAuditEventsQuery query)
    {
        if (query.Page < 1 || query.PageSize < 1 || query.PageSize > 100)
            return BadRequest("Invalid paging parameters.");

        var response = await _auditService.GetAuditEventsAsync(query);

        return Ok(response);
    }
}