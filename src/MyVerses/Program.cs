using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MyVerses.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.AddCosmosDbContext<MyVersesDbContext>("cosmos", "myverses");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Ensure Cosmos DB database and container exist in local development.
    // The emulator's pgcosmos extension can still be initializing even after Aspire
    // reports it healthy, so retry with backoff on ServiceUnavailable.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MyVersesDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    for (var attempt = 1; attempt <= 10; attempt++)
    {
        try
        {
            await db.Database.EnsureCreatedAsync();
            break;
        }
        catch (Microsoft.Azure.Cosmos.CosmosException ex)
            when (ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
        {
            if (attempt == 10) throw;
            logger.LogWarning("Cosmos emulator not ready (attempt {Attempt}/10), retrying in {Delay}s...",
                attempt, attempt * 2);
            await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
