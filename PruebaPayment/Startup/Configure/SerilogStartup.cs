using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace PruebaPayment.Startup.Configure;

internal static class SerilogStartup
{
    internal static void AddAndConfigureSerilog(WebApplicationBuilder builder)
    {
        string logsDirectory = Path.Combine(builder.Environment.ContentRootPath, "logs");
        Directory.CreateDirectory(logsDirectory);

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Combine(logsDirectory, "log_.txt"),
                rollingInterval: RollingInterval.Day)
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = "http://otel-endpoint";
                options.Protocol = OtlpProtocol.HttpProtobuf;
            })
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddSerilog();
    }
}