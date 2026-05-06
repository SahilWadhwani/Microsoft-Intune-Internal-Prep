using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EndpointGuardian.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    private readonly IDeviceComplianceService _deviceComplianceService;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(IDeviceService deviceService, IDeviceComplianceService deviceComplianceService, ILogger<DevicesController> logger)
    {
        _deviceService = deviceService;
        _deviceComplianceService = deviceComplianceService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "CanWriteDevices")]
    public async Task<ActionResult<DeviceResponse>> Create(CreateDeviceRequest request)
    {
        var result = await _deviceService.CreateDeviceAsync(request);

        if (result is null)
        {
            return Conflict("A device with this ID already exists.");
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize(Policy = "CanReadDevices")]  
    public async Task<ActionResult<PagedDeviceResponse>> GetDevices([FromQuery] GetDevicesQuery query)
    {
        if (query.page < 1 || query.PageSize < 1 || query.PageSize > 100)
        {
            return BadRequest("Invalid paging parameters.");
        }
        var response = await _deviceService.GetDevicesAsync(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "CanReadDevices")]
    public async Task<ActionResult<DeviceResponse>> GetById(string id)
    {
        _logger.LogInformation("Received request to fetch device with id {DeviceId}", id);

        var device = await _deviceService.GetDeviceByIdAsync(id);

        if (device is null)
        {
            return NotFound();
        }

        return Ok(device);
    }

    
    [HttpPost("{id}/checkin")]
    public ActionResult<ManagedDevice> CheckIn(string id, CheckInDeviceRequest request)
    {
        var device = _deviceService.CheckInDevice(id, request);

        if (device is null)
        {
            return NotFound();
        }

        return Ok(device);
    }

    [HttpPost("{id}/evaluate")]
    [Authorize(Policy = "CanRunEvaluations")]
    public ActionResult<ComplianceEvaluationResult> Evaluate(string id)
    {
        var result = _deviceService.EvaluateDevice(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpsPost("{id}/evaluations")]
    public async Task<ActionResult<DeviceComplianceEvaluationResult>> CreateEvaluation(string id)
    {
        var result = await _deviceComplianceService.EvaluateDevice(id);

        if (result is null)
        {
            return NotFound();
        }

        return DayOfWeek(result);
    }
}