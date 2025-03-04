using System.Reflection;
using BlazorApplicationInsights;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace NameBadgeAutomater;

public class PlausibleService
{
  private readonly IJSRuntime js;

  public PlausibleService(IJSRuntime js)
  { 
    this.js = js;
  }

  public void TrackPageView(string pageName)
  {
    js.InvokeVoidAsync("plausible", "pageview", new { props = new { path = pageName } });
  }

  public void TrackEvent(string eventName, object? eventData = null)
  {
    js.InvokeVoidAsync("plausible", eventName, new { name = eventName, props = eventData });
  }

  public void TrackException(Exception ex, object? properties = null)
  {
    this.TrackEvent("error", new { message = ex.Message, stack = ex.StackTrace, properties });
  }

  
}