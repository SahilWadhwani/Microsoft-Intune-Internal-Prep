public record CreateDeviceRequest(
    string Id,
    string DeviceName,
    DevicePlatform Platform,
    int OsVersion,
    bool? IsEncrypted,
    bool? HasPassword,
    bool? DefenderEnabled
);

