using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EndpointGuardian.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public ActionResult<List<ManagedDevice>> GetAll()
    {
        return Ok(_deviceService.GetAllDevices());
    }

    [HttpGet("{id}")]
    public ActionResult<ManagedDevice> GetById(string id)
    {
        var device = _deviceService.GetDeviceById(id);

        if (device is null)
        {
            return NotFound();
        }

        return Ok(device);
    }

    [HttpPost]
    public ActionResult<ManagedDevice> Create(CreateDeviceRequest request)
    {
        var device = _deviceService.RegisterDevice(request);

        if (device is null)
        {
            return BadRequest("A device with this ID already exists.");
        }

        return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
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
}