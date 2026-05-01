namespace EndpointGuardian.Api.Models.Entities;

public record DevicePostureSnapshot(

    bool? IsEncrypted,

    bool? HasPassword,

    bool? DefenderEnabled,

    DateTime CapturedAtUtc

);