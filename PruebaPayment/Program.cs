using PruebaPayment.Startup;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

StartupHandler.ConfigureStartup(builder);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await StartupHandler.UseStartup(app);

app.Run();
