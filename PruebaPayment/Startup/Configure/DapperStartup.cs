using Microsoft.Data.SqlClient;
using Npgsql;

namespace PruebaPayment.Startup.Configure;

public static class DapperStartup
{
    public static readonly string ConnectionStringName = "payment-db";
    public static void AddDapperSqlConnection(WebApplicationBuilder builder)
    {
        string dbConnectionString = builder.Configuration.GetConnectionString(ConnectionStringName) ?? throw new Exception($"Db ConnectionString {ConnectionStringName} is not configured");
        var dataSource = new NpgsqlDataSourceBuilder(dbConnectionString).Build();
        builder.Services.AddSingleton(dataSource);
    }
}
