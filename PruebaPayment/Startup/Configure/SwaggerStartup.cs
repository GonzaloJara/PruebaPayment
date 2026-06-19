using Microsoft.OpenApi;

namespace PruebaPayment.Startup.Configure;

public static class SwaggerStartup
{
    public static void AddSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDesc => apiDesc.First());

            options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
        });
    }

    public static void UseSwagger(WebApplication app)
    {
        app.MapSwagger().AllowAnonymous();
        app.UseSwaggerUI(c =>
        {
            c.EnablePersistAuthorization();
            c.EnableTryItOutByDefault();
        });
    }
}
