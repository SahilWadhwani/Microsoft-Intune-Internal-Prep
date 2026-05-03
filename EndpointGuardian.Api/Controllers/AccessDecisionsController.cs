using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EndpointGuardian.Api.Controllers;

[ApiController]
[Route("api/access-decisions")]
public class AccessDecisionsController : ControllerBase
{
    private readonly IAccessDecisionService _accessDecisionService;
    private readonly ILogger<AccessDecisionsController> _logger;

    public AccessDecisionsController(
        IAccessDecisionService accessDecisionService,
        ILogger<AccessDecisionsController> logger)
    {
        _accessDecisionService = accessDecisionService;
        _logger = logger;
    }

    [HttpPost]
    public ActionResult<AccessDecisionResponse> CreateDecision(
        CreateAccessDecisionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) ||
            string.IsNullOrWhiteSpace(request.DeviceId) ||
            string.IsNullOrWhiteSpace(request.Resource))
        {
            return BadRequest("UserId, DeviceId, and Resource are required.");
        }

        var decision = _accessDecisionService.CreateDecision(request);

        if (decision is null)
        {
            return NotFound("Device was not found.");
        }

        return Ok(decision);
    }
}