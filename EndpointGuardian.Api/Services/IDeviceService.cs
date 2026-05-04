using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IDeviceService
{
    Task<DeviceResponse?> CreateDeviceAsync(CreateDeviceRequest request);

    Task<DeviceResponse?> GetDeviceByIdAsync(string id);

    Task<PagedDevicesResponse> GetDevicesAsync(GetDevicesQuery query);
}