using System.Reflection;
using BlazorApplicationInsights;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace NameBadgeAutomater;

public class SupportService
{
  private readonly ISyncLocalStorageService localStorage;
  private readonly NavigationManager navigationManager;
  
  private const string DisableTelemetryStorageKey = "disableTelemetry";

  public SupportService(ISyncLocalStorageService localStorage, NavigationManager navigationManager)
  {
    this.localStorage = localStorage;
    this.navigationManager = navigationManager;
  }

  public async Task<string> GetAboutDetails(string newLineSeporator) => $"""
    Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}
    Full Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}
    """.ReplaceLineEndings(newLineSeporator);

  public async Task<string> GetMailToHref() => $"mailto:github.support@tdashworth.uk?subject=Name Badge Automater&body={ await GetEmailBody()}";

  private async Task<string> GetEmailBody() => $"""
    Hi,

    ...

    { await GetAboutDetails("%0D%0A") }
    """.ReplaceLineEndings("%0D%0A");

  public bool IsTelemetryEnabled => !localStorage.GetItem<bool>(DisableTelemetryStorageKey);

  public void DisableTelemetry()
  {
    localStorage.SetItem(DisableTelemetryStorageKey, true);
    navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
  }

  public void EnableTelemetry()
  {
    localStorage.SetItem(DisableTelemetryStorageKey, false);
    navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
  }
}