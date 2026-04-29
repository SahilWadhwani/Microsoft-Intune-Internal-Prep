public record CreateDeviceRequest(
    string DeviceName,
    DevicePlatform Platform,
    int OsVersion,
    bool? IsEncrypted,
    bool? HasPassword,
    bool? DefenderEnabled
);