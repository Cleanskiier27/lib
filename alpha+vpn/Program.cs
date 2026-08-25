var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/activity", () => Results.Ok(Array.Empty<object>()));

app.MapGet("/api/license/status", () => Results.Ok(new { status = "inactive", expiresAt = (string?)null }));

// TODO: replace with real signal strength telemetry from the VPN daemon.
app.MapGet("/api/signal", () => Results.Ok(new { strength = 0, status = "warn" }));

app.MapPost("/api/license/activate", (LicenseActivationRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Key))
    {
        return Results.BadRequest(new { message = "License key is required." });
    }

    // TODO: replace with real license verification against a licensing service.
    return Results.Ok(new { status = "active" });
});

app.Run();

record LicenseActivationRequest(string Key);
