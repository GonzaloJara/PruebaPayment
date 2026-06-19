using System.Reflection;

namespace PruebaPayment.Startup.DI;

public interface IServiceRegistration
{
    void RegisterServices(WebApplicationBuilder builder);
}

public static class RegisterServicesExtention
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder, Assembly assembly)
    {
        var serviceTypes = assembly
            .GetTypes()
            .Where(t => typeof(IServiceRegistration).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in serviceTypes)
        {
            var instance = (IServiceRegistration)Activator.CreateInstance(type)!;
            instance.RegisterServices(builder);
        }

        return builder;
    }
}