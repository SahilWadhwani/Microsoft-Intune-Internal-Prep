public enum DevicePlatform
{
    Unknown,
    Wondows,
    Mac,
    IOS,
    Android
}

public class ManagedDevice
{
    public string Id {get; set;} = "";
    public string DeviceName {get; set;} = "";
    public DevicePlatform PLatform {get; set;}
    public int OsVersion {get; set;}

    public bool? IsEncrypted {get; set;}

    public bool? HasPassword {get; set;}

    public bool? DefenderEnabled {get; set;}

    public DateTime LastCheckInUtc {get; set;}

}

