using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient("serviceA", c => c.BaseAddress = new Uri("http://localhost:5002/graphql"));

builder.Services
    .AddHttpClient("serviceB", c => c.BaseAddress = new Uri("http://localhost:5003/graphql"));

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect("localhost"))
    .AddGraphQLServer()
    .AddRemoteSchemasFromRedis("gateway", sp => sp.GetRequiredService<ConnectionMultiplexer>());

var app = builder.Build();

app.MapGraphQL();

app.Run();