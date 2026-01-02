using BlazorApplicationInsights.Interfaces;
using BlazorApplicationInsights.Models;

namespace NameBadgeAutomater
{
    public static class ApplicationInsightsExtensions
    {
        public static async Task TrackEvent(this IApplicationInsights appInsights, string eventName, Dictionary<string, object?>? properties = null)
        {
            await appInsights.TrackEvent(new EventTelemetry
            {
                Name = eventName,
                Properties = properties
            });
            await appInsights.Flush();
        }

        public static async Task TrackPageView(this IApplicationInsights appInsights, string pageName, Dictionary<string, object?>? properties = null)
        {
            await appInsights.TrackPageView(new PageViewTelemetry
            {
                Name = pageName,
                Properties = properties
            });
            await appInsights.Flush();
        }

        public static async Task TrackException(this IApplicationInsights appInsights, Exception exception, Dictionary<string, object?>? properties = null)
        {
            await appInsights.TrackException(new ExceptionTelemetry
            {
                Exception = new Error
                {
                    Name = exception.GetType().Name,
                    Message = exception.Message,
                    Stack = exception.ToString()
                },
                Properties = properties
            });
            await appInsights.Flush();
        }
    }
}