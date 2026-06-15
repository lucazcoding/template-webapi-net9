namespace LACUNATECH_challenge.Models;

public class ProbeListResponse
{
    public string Code { get; set; }
    public string? Message { get; set; }
    public List<Probe>? Probes { get; set; }
}