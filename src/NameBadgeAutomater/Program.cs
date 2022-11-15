using BlazorApplicationInsights;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NameBadgeAutomater;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<AppState>();
builder.Services.AddSingleton<PowerPointTemplateService>();

builder.Services.AddBeforeUnload();
builder.Services.AddBlazorApplicationInsights();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
