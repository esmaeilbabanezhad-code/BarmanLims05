using Barman.Persistence.Contexts;
using Barman.Persistence.DependencyInjection;
using Barman.Persistence.Seed;

using Barman.Infrastructure.DependencyInjection;

using BarmanLims.Components;

using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================
// Services
// =============================

builder.Services.AddMudServices();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddInfrastructure();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// =============================

var app = builder.Build();

// =============================
// Database Initialization
// =============================

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await ApplicationDbInitializer.InitializeAsync(db);
}

// =============================
// Pipeline
// =============================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();