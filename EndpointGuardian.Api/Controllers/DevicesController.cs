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
    public ActionResult<DeviceResponse> Create(CreateDeviceRequest request)
    {
        var result = _deviceService.CreateDevice(request);

        if (result is null)
        {
            return Conflict("A device with this ID already exists.");
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public ActionResult<PagedDeviceResponse> GetDevices([FromQuery] GetDevicesQuery query)
    {
        if (query.page < 1 || query.PageSize < 1 || query.PageSize > 100)
        {
            return BadRequest("Invalid paging parameters.");
        }
        var response = _deviceService.GetDevices(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public ActionResult<DeviceResponse> GetById(string id)
    {
        _logger.LogInformation("Received request to fetch device with id {DeviceId}", id);

        var device = _deviceService.GetDeviceById(id);

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
    public ActionResult<DeviceComplianceEvaluationResult> CreateEvaluation(string id)
    {
        var result = _deviceComplianceService.EvaluateDevice(id);

        if (result is null)
        {
            return NotFound();
        }

        return DayOfWeek(result);
    }
}