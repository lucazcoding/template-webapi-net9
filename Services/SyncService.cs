using LACUNATECH_challenge.Helpers;
using LACUNATECH_challenge.Models;

namespace LACUNATECH_challenge.Services;

public class SyncService
{
    private readonly LumaApiClient _api;
    private readonly ProbeClockStore _store; // ← NOVO

    private const long FiveMillisecondsTicks = 5 * 10_000;

    public SyncService(LumaApiClient api, ProbeClockStore store) // ← NOVO
    {
        _api = api;
        _store = store;
    }

    public async Task SyncAllAsync(List<Probe> probes, string token)
    {
        foreach (var probe in probes)
        {
            Console.WriteLine($"[Sync] Iniciando sincronização: {probe.Name}");
            await SyncProbeAsync(probe, token);
            Console.WriteLine($"[Sync] {probe.Name} sincronizada!");
        }
    }

    private async Task SyncProbeAsync(Probe probe, string token)
    {
        if (!_store.Contains(probe.Id))
            _store.Set(probe.Id, new ProbeClockState());

        long? previousOffset = null;

        while (true)
        {
            var (syncData, t0, t3) = await _api.SyncAsync(probe.Id, token);

            if (syncData.Code != "Success" || syncData.T1 is null || syncData.T2 is null)
                throw new Exception($"Sync falhou para {probe.Name}. Code: {syncData.Code}");

            long t1 = TimestampDecoder.Decode(syncData.T1, probe.Encoding);
            long t2 = TimestampDecoder.Decode(syncData.T2, probe.Encoding);

            long fator1 = t1 - t0;
            long fator2 = t2 - t3;
            long newOffset = (fator1 + fator2) / 2;
            long roundTrip = (t3 - t0) - (t2 - t1);

            var state = _store.Get(probe.Id);
            state.TimeOffset = newOffset;
            state.LastRoundTrip = roundTrip;

            Console.WriteLine($"[Sync] {probe.Name} | newOffset: {newOffset} ticks | roundTrip: {roundTrip} ticks");

            if (previousOffset.HasValue &&
                Math.Abs(newOffset - previousOffset.Value) < FiveMillisecondsTicks)
            {
                break;
            }

            previousOffset = newOffset;
        }
    }
    // Esses dois métodos não mudam nada — só delegam pro store
    public long GetProbeNow(string probeId)
    {
        var state = _store.Get(probeId);
        return DateTimeOffset.UtcNow.Ticks + state.TimeOffset;
    }

    public long GetRoundTrip(string probeId) =>
        _store.Get(probeId).LastRoundTrip;
}