public class CompliancePolicy
{
    public string Id {get; set;} = "";
    public string Name {get; set;} = "";

    public int MinimumOsVersion {get; set;}
    public bool RequireEncryption {get; set;}
    public bool RequirePassword {get; set;}

    public bool RequireDefender {get; set;}

    public DateTime MaxCheckInAgeHours {get; set;}

}

