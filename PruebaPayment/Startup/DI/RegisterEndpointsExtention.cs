using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace PruebaPayment.Startup.DI;

public interface IEndpointRegistration
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

public static class RegisterEndpointsExtention
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = [.. assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpointRegistration)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpointRegistration), type))];

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
        this WebApplication app)
    {
        IEnumerable<IEndpointRegistration> endpoints = app.Services
            .GetRequiredService<IEnumerable<IEndpointRegistration>>();

        foreach (IEndpointRegistration endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
