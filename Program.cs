using System.Net.Http.Headers;
using LACUNATECH_challenge;
using LACUNATECH_challenge.Models;
using LACUNATECH_challenge.Services;
using LACUNATECH_challenge.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<LumaApiClient>(client =>
{
    client.BaseAddress = new Uri("https://luma.lacuna.cc/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// Apenas o estado fica Singleton
builder.Services.AddSingleton<ProbeClockStore>(); // guarda os offsets
builder.Services.AddScoped<SyncService>();      // lógica
builder.Services.AddScoped<JobService>();       // lógica
builder.Services.AddHostedService<LumaWorker>();

var app = builder.Build();
app.Run();