using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Services;
using Barman.Application.Interfaces.Services.Reporting;
using Barman.Application.Services;
using Barman.Application.Versioning;
using Barman.Infrastructure.DependencyInjection;
using Barman.Infrastructure.Services.Reporting;
using Barman.Infrastructure.Services.Versioning;
using Barman.Persistence.Contexts;
using Barman.Persistence.DependencyInjection;
using Barman.Persistence.Seed;
using Barman.Persistence.Services;
using BarmanLims.Components;
using BarmanLims.Login;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using System.Diagnostics;


// =============================
// Web Application Configuration
// =============================

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = AppContext.BaseDirectory
    });

// Fixed URL for local Barman LIMS installation
builder.WebHost.UseUrls("http://localhost:5062");


// =============================
// Configuration Diagnostics
// =============================

Console.WriteLine();
Console.WriteLine("========================================");
Console.WriteLine(" CONFIGURATION PROVIDERS DIAGNOSTIC");
Console.WriteLine("========================================");

Console.WriteLine(
    $"BaseDirectory: {AppContext.BaseDirectory}");

Console.WriteLine(
    $"CurrentDirectory: {Environment.CurrentDirectory}");

Console.WriteLine(
    $"Environment: {builder.Environment.EnvironmentName}");

var configurationRoot =
    (IConfigurationRoot)builder.Configuration;

foreach (var provider in configurationRoot.Providers)
{
    Console.WriteLine(
        $"Provider: {provider.GetType().FullName}");
}

Console.WriteLine(
    $"DefaultConnection: [{builder.Configuration.GetConnectionString("DefaultConnection")}]");

Console.WriteLine("========================================");
Console.WriteLine();


// =============================
// Services
// =============================

builder.Services.AddMudServices();

builder.Services.AddPersistence(
    builder.Configuration);

builder.Services.AddHostedService<
    DatabaseBackupScheduler>();

builder.Services.AddInfrastructure();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";

        options.ExpireTimeSpan =
            TimeSpan.FromHours(8);

        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

builder.Services.AddScoped<
    IReportExportService,
    ReportExportService>();


// =============================
// Application Update Service
// =============================

builder.Services.AddHttpClient<
    IUpdateService,
    UpdateService>();


// =============================
// Build Application
// =============================

var app = builder.Build();


// =============================
// Database Upgrade Check
// =============================

using (var scope = app.Services.CreateScope())
{
    var upgradeService =
        scope.ServiceProvider
            .GetRequiredService<
                IApplicationDatabaseUpgradeService>();

    var backupService =
        scope.ServiceProvider
            .GetRequiredService<
                IDatabaseBackupService>();

    var currentMigration =
        await upgradeService
            .GetCurrentMigrationAsync();

    var pendingMigrations =
        await upgradeService
            .GetPendingMigrationsAsync();

    Console.WriteLine();
    Console.WriteLine("========================================");
    Console.WriteLine(
        " Barman LIMS Database Upgrade Check");
    Console.WriteLine("========================================");

    Console.WriteLine(
        $"Current database migration: " +
        $"{currentMigration ?? "(none)"}");

    if (pendingMigrations.Count == 0)
    {
        Console.WriteLine(
            "Database is up to date.");

        Console.WriteLine(
            "No database migration is required.");
    }
    else
    {
        Console.WriteLine();
        Console.WriteLine(
            "Pending database migrations detected:");

        foreach (var migration in pendingMigrations)
        {
            Console.WriteLine(
                $" - {migration}");
        }

        Console.WriteLine();
        Console.WriteLine(
            "Creating database backup before migration...");

        var backupPath =
            await backupService
                .CreateBackupAsync(
                    "BeforeMigration");

        Console.WriteLine(
            "Database backup created successfully:");

        Console.WriteLine(
            backupPath);

        Console.WriteLine();
        Console.WriteLine(
            "Applying database migrations...");

        await upgradeService
            .ApplyMigrationsAsync();

        Console.WriteLine();
        Console.WriteLine(
            "Database migrations applied successfully.");
    }

    Console.WriteLine(
        "========================================");

    Console.WriteLine();
}


// =============================
// Database Initialization
// =============================

using (var scope = app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
            .GetRequiredService<
                ApplicationDbContext>();

    await ApplicationDbInitializer
        .InitializeAsync(db);
}


// =============================
// HTTP Pipeline
// =============================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

// IMPORTANT:
// Do NOT use HTTPS redirection here.
// The standalone local EXE uses HTTP
// on localhost:5062.

app.UseAuthentication();

app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapLoginEndpoints();


// =============================
// Start Barman LIMS
// =============================

var applicationUrl =
    "http://localhost:5062";

Console.WriteLine();
Console.WriteLine("========================================");
Console.WriteLine(" Barman LIMS Web Server");
Console.WriteLine("========================================");

Console.WriteLine(
    $"Starting server: {applicationUrl}");

Console.WriteLine();


// =============================
// Start Kestrel
// =============================

await app.StartAsync();

Console.WriteLine(
    "Barman LIMS server started.");

Console.WriteLine(
    "Waiting for web server to become ready...");


// =============================
// Wait Until HTTP Is Ready
// =============================

using (var httpClient = new HttpClient())
{
    httpClient.Timeout =
        TimeSpan.FromSeconds(2);

    var ready = false;

    for (var i = 0; i < 40; i++)
    {
        try
        {
            using var response =
                await httpClient.GetAsync(
                    applicationUrl);

            if (response.IsSuccessStatusCode)
            {
                ready = true;
                break;
            }
        }
        catch
        {
            // Server is not ready yet.
        }

        await Task.Delay(250);
    }


    // =============================
    // Open Browser Automatically
    // =============================

    if (ready)
    {
        Console.WriteLine(
            "Web server is ready.");

        Console.WriteLine(
            $"Opening browser: {applicationUrl}");

        try
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = applicationUrl,
                    UseShellExecute = true
                });

            Console.WriteLine(
                "Browser started successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "Unable to open browser automatically:");

            Console.WriteLine(
                ex.Message);

            Console.WriteLine();

            Console.WriteLine(
                $"Please open manually: {applicationUrl}");
        }
    }
    else
    {
        Console.WriteLine(
            "WARNING: Web server did not respond " +
            "within the expected time.");

        Console.WriteLine();

        Console.WriteLine(
            $"Please open manually: {applicationUrl}");
    }
}


// =============================
// Keep Application Running
// =============================

await app.WaitForShutdownAsync();