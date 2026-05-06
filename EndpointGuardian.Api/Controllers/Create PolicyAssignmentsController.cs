using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EndpointGuardian.Api.Controllers;

[ApiController]
public class PolicyAssignmentsController : ControllerBase
{
    private readonly IPolicyAssignmentService _assignmentService;
    private readonly ILogger<PolicyAssignmentsController> _logger;

    public PolicyAssignmentsController(
        IPolicyAssignmentService assignmentService,
        ILogger<PolicyAssignmentsController> logger)
    {
        _assignmentService = assignmentService;
        _logger = logger;
    }

    [HttpPost("api/policies/{policyId}/assignments")]
    [Authorize(Policy = "CanAssignPolicies")]
    public ActionResult<PolicyAssignmentResponse> CreateAssignment(
        string policyId,
        CreatePolicyAssignmentRequest request)
    {
        var assignment = _assignmentService.CreateAssignment(policyId, request);

        if (assignment is null)
        {
            return BadRequest("Assignment could not be created.");
        }

        return CreatedAtAction(
            nameof(GetAssignmentById),
            new { assignmentId = assignment.Id },
            assignment);
    }

    [HttpGet("api/policies/{policyId}/assignments")]
    [Authorize(Policy = "CanReadPolicies")]
    public ActionResult<PagedPolicyAssignmentsResponse> GetAssignmentsForPolicy(
        string policyId,
        [FromQuery] GetPolicyAssignmentsQuery query)
    {
        if (query.Page < 1 || query.PageSize < 1 || query.PageSize > 100)
        {
            return BadRequest("Invalid paging parameters.");
        }

        var response = _assignmentService.GetAssignmentsForPolicy(policyId, query);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpGet("api/assignments/{assignmentId}")]
    [Authorize(Policy = "CanReadPolicies")]
    public ActionResult<PolicyAssignmentResponse> GetAssignmentById(string assignmentId)
    {
        var assignment = _assignmentService.GetAssignmentById(assignmentId);

        if (assignment is null)
        {
            return NotFound();
        }

        return Ok(assignment);
    }
}