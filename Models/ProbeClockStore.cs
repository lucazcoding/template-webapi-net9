namespace LACUNATECH_challenge.Models;

public class ProbeClockStore
{
    private readonly Dictionary<string, ProbeClockState> _clocks = new();

    public void Set(string probeId, ProbeClockState state) =>
        _clocks[probeId] = state;

    public ProbeClockState Get(string probeId) =>
        _clocks.TryGetValue(probeId, out var state)
            ? state
            : throw new Exception($"Sonda {probeId} não sincronizada!");

    public bool Contains(string probeId) => 
        _clocks.ContainsKey(probeId);
    
    
    public void Clear() => _clocks.Clear();
}