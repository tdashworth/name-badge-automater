using System.Reflection;
using BlazorApplicationInsights;
using BlazorApplicationInsights.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NameBadgeAutomater;

// I love you, more than I can put into words, more than I even know.
// All I can do is write code and hope it can make the world a better place.
// ...and put a smile on your face everytime you execute it. 
// Your smile brightens my whole world, right as I sit across the table writing this. 

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<AppState>();
builder.Services.AddScoped<PowerPointTemplateService>();
builder.Services.AddScoped<NameParserService>();
builder.Services.AddScoped<SupportService>();

builder.Services.AddBeforeUnload();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazorApplicationInsights(config =>
    {
        config.ConnectionString = "InstrumentationKey=00fb8745-5453-4e32-9e45-ff1395a50b79;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=24d28cde-9fb2-4da0-9167-268b7fdafb3b";
        config.EnableCorsCorrelation = true;
        config.EnableRequestHeaderTracking = true;
        config.EnableResponseHeaderTracking = true;
        // config.DisableTelemetry = builder.HostEnvironment.IsDevelopment();
        // Console.WriteLine($"Telemetry Disabled: {config.DisableTelemetry}");
    },
    async applicationInsights =>
    {
        var telemetryItem = new TelemetryItem()
        {
            Data = new Dictionary<string, object?>()
            {
                { "appVersion", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "unknown" }
            }
        };

        await applicationInsights.AddTelemetryInitializer(telemetryItem);
    });
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();

