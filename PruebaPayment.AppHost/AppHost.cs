var builder = DistributedApplication.CreateBuilder(args);

var dbparam = builder.AddParameter("postgres");
var mqparam = builder.AddParameter("rabbitmq");

var postgres = builder.AddPostgres("payment-postgres", dbparam, dbparam)
    .WithDataVolume()
    .WithPgAdmin();

var db = postgres.AddDatabase("payment-db");

var mq = builder.AddRabbitMQ("payment-mq", mqparam, mqparam)
    .WithDataVolume()
    .WithManagementPlugin();

var redis = builder.AddRedis("payment-cache");

var adq = builder.AddProject<Projects.PruebaPayment_Adq>("payment-adq");

builder.AddProject<Projects.PruebaPayment>("payment-api")
    .WithReference(db)
    .WithReference(redis)
    .WithReference(mq)
    .WithReference(adq)
    .WaitFor(mq)
    .WaitFor(db);

builder.Build().Run();
