using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IDeviceService
{
    DeviceResponse? CreateDevice(CreateDeviceRequest request);

    DeviceResponse? GetDeviceById(string id);

    PagedDevicesResponse GetDevices(GetDevicesQuery query);
}