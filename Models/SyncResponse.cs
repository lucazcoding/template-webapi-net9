namespace LACUNATECH_challenge.Models;

public class SyncResponse
{
    public string Code { get; set; }
    public string? Message { get; set; }
    public string? T1 { get; set; }  // hora da probe ao receber
    public string? T2 { get; set; }  // hora da probe ao responder
}