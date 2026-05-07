using EndpointGuardian.Api.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EndpointGuardian.Api.Controllers;

[ApiController]
[Route("api/system/scheduled-compliance")]
public class ScheduledComplianceController : ControllerBase
{
    private readonly ScheduledComplianceOptions _options;

    public ScheduledComplianceController(
        IOptions<ScheduledComplianceOptions> options)
    {
        _options = options.Value;
    }

    [HttpGet]
    [Authorize(Policy = "CanReadAudit")]
    public ActionResult<ScheduledComplianceOptions> GetStatus()
    {
        return Ok(_options);
    }
}