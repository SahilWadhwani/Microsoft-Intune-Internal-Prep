using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EndpointGuardian.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyService _policyService;
    private readonly ILogger<PoliciesController> _logger;

    public PoliciesController(
        IPolicyService policyService,
        ILogger<PoliciesController> logger)
    {
        _policyService = policyService;
        _logger = logger;
    }

    [HttpPost]
    public ActionResult<PolicyResponse> Create(CreatePolicyRequest request)
    {
        var policy = _policyService.CreatePolicy(request);

        if (policy is null)
        {
            return BadRequest("Invalid policy request.");
        }

        return CreatedAtAction(nameof(GetById), new { id = policy.Id }, policy);
    }

    [HttpGet]
    public ActionResult<PagedPoliciesResponse> GetPolicies([FromQuery] GetPoliciesQuery query)
    {
        if (query.Page < 1 || query.PageSize < 1 || query.PageSize > 100)
        {
            return BadRequest("Invalid paging parameters.");
        }

        var response = _policyService.GetPolicies(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public ActionResult<PolicyResponse> GetById(string id)
    {
        var policy = _policyService.GetPolicyById(id);

        if (policy is null)
        {
            return NotFound();
        }

        return Ok(policy);
    }
}