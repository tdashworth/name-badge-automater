using BlazorApplicationInsights;
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
builder.Services.AddBlazorApplicationInsights();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
