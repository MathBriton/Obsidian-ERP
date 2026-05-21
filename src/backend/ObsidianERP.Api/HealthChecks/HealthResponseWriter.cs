using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ObsidianERP.Api.HealthChecks;

public static class HealthResponseWriter
{
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var payload = new
        {
            status = report.Status.ToString(),
            timestamp = DateTimeOffset.UtcNow,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString()
            })
        };

        return context.Response.WriteAsJsonAsync(payload);
    }
}
