using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NameBadgeAutomater;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(new AppState());
builder.Services.AddSingleton(new PowerPointTemplateService());

builder.Services.AddBeforeUnload();

await builder.Build().RunAsync();
