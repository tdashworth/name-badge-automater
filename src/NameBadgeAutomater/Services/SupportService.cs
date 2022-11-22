using System.Reflection;
using BlazorApplicationInsights;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace NameBadgeAutomater;

public class SupportService
{
  private readonly IApplicationInsights appInsights;
  private readonly ISyncLocalStorageService localStorage;
  private readonly NavigationManager navigationManager;
  
  private const string DisableTelemetryStorageKey = "disableTelemetry";

  public SupportService(IApplicationInsights appInsights, ISyncLocalStorageService localStorage, NavigationManager navigationManager)
  {
    this.appInsights = appInsights;
    this.localStorage = localStorage;
    this.navigationManager = navigationManager;
  }

  public async Task<string> GetAboutDetails(string newLineSeporator) => $"""
    App version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}
    User ID: {await appInsights.GetUserId()}
    Session ID: {await appInsights.GetSessionId()}
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
    appInsights.TrackEvent("TelemetryDisabled");
    navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
  }

  public void EnableTelemetry()
  {
    localStorage.SetItem(DisableTelemetryStorageKey, false);
    navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
  }
}