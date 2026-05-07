using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Security;
using EndpointGuardian.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EndpointGuardian.Api.Controllers;

[ApiController]
public class RemediationActionsController : ControllerBase
{
    private readonly IRemediationService _remediationService;

    public RemediationActionsController(IRemediationService remediationService)
    {
        _remediationService = remediationService;
    }

    [HttpPost("api/devices/{deviceId}/remediation-actions")]
    [Authorize(Policy = "CanExecuteRemediation")]
    public async Task<ActionResult<RemediationActionResponse>> Create(
        string deviceId,
        CreateRemediationActionRequest request)
    {
        var actor = User.GetActorId();

        var action = await _remediationService.CreateRemediationActionAsync(
            deviceId,
            request,
            actor);

        if (action is null)
            return BadRequest("Remediation action could not be created.");

        return CreatedAtAction(
            nameof(GetById),
            new { actionId = action.Id },
            action);
    }

    [HttpGet("api/devices/{deviceId}/remediation-actions")]
    [Authorize(Policy = "CanExecuteRemediation")]
    public async Task<ActionResult<PagedRemediationActionsResponse>> GetForDevice(
        string deviceId,
        [FromQuery] GetRemediationActionsQuery query)
    {
        if (query.Page < 1 || query.PageSize < 1 || query.PageSize > 100)
            return BadRequest("Invalid paging parameters.");

        var response = await _remediationService.GetRemediationActionsForDeviceAsync(
            deviceId,
            query);

        if (response is null)
            return NotFound();

        return Ok(response);
    }

    [HttpGet("api/remediation-actions/{actionId}")]
    [Authorize(Policy = "CanExecuteRemediation")]
    public async Task<ActionResult<RemediationActionResponse>> GetById(string actionId)
    {
        var action = await _remediationService.GetByIdAsync(actionId);

        if (action is null)
            return NotFound();

        return Ok(action);
    }

    [HttpPatch("api/remediation-actions/{actionId}/status")]
    [Authorize(Policy = "CanExecuteRemediation")]
    public async Task<ActionResult<RemediationActionResponse>> UpdateStatus(
        string actionId,
        UpdateRemediationActionStatusRequest request)
    {
        var actor = User.GetActorId();

        try
        {
            var action = await _remediationService.UpdateStatusAsync(
                actionId,
                request,
                actor);

            if (action is null)
                return NotFound();

            return Ok(action);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}