namespace LACUNATECH_challenge.Models;

public class JobResponse
{
    public string Code { get; set; }
    public string? Message { get; set; }
    public Job? Job { get; set; }
}