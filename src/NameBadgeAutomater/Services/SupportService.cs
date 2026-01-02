using System.Reflection;
using BlazorApplicationInsights;
using BlazorApplicationInsights.Interfaces;
using BlazorApplicationInsights.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace NameBadgeAutomater;

public class SupportService
{
  private readonly IApplicationInsights appInsights;

  public SupportService(IApplicationInsights appInsights, ISyncLocalStorageService localStorage, NavigationManager navigationManager)
  {
    this.appInsights = appInsights;
  }

  public async Task<string> GetAboutDetails(string newLineSeporator) => $"""
    Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}
    Full Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown"}
    User ID: {(await appInsights.Context()).User.Id}
    Session ID: {(await appInsights.Context()).SessionManager.AutomaticSession.Id}
    """.ReplaceLineEndings(newLineSeporator);

  public async Task<string> GetMailToHref() => $"mailto:github.support@tdashworth.uk?subject=Name Badge Automater&body={ await GetEmailBody()}";

  private async Task<string> GetEmailBody() => $"""
    Hi,

    ...

    { await GetAboutDetails("%0D%0A") }
    """.ReplaceLineEndings("%0D%0A");
}