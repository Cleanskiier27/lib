var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/activity", () => Results.Ok(Array.Empty<object>()));

app.MapGet("/api/license/status", () => Results.Ok(new { status = "inactive", expiresAt = (string?)null }));

// TODO: replace with real connection/session data from the VPN daemon.
app.MapGet("/api/connections", () => Results.Ok(Array.Empty<object>()));

app.MapPost("/api/license/activate", (LicenseActivationRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Key))
    {
        return Results.BadRequest(new { message = "License key is required." });
    }

    // TODO: replace with real license verification against a licensing service.
    return Results.Ok(new { status = "active" });
});

// TODO: back with a real session store; in-memory only for local dev.
var unidentifiedUsers = new List<UnidentifiedUser>();

app.MapGet("/api/unidentified-users", () => Results.Ok(unidentifiedUsers));

app.MapPost("/api/unidentified-users/{id}/block", (string id) =>
{
    var user = unidentifiedUsers.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();
    user.Status = "blocked";
    return Results.Ok(user);
});

app.MapPost("/api/unidentified-users/{id}/escalate", (string id) =>
{
    var user = unidentifiedUsers.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();
    user.Status = "escalated";
    return Results.Ok(user);
});

app.MapPost("/api/unidentified-users/{id}/dismiss", (string id) =>
{
    var user = unidentifiedUsers.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();
    user.Status = "dismissed";
    return Results.Ok(user);
});

app.Run();

record LicenseActivationRequest(string Key);

class UnidentifiedUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string IpAddress { get; set; } = "";
    public string FirstSeen { get; set; } = "";
    public string Location { get; set; } = "";
    public int RiskScore { get; set; }
    public string Status { get; set; } = "unidentified";
}

