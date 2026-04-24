using System;

namespace EndpointGuardian.Api.Models;

public record CheckInDeviceRequest(
    bool? IsEncrypted,
    bool? HasPassword,
    bool? DefenderEnabled
);