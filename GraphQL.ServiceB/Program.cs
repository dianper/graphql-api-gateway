using GraphQL.ServiceB.Types;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect("localhost"))
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .InitializeOnStartup()
    .PublishSchemaDefinition(c => c
        .SetName("serviceB")
        .PublishToRedis("gateway", sp => sp.GetRequiredService<ConnectionMultiplexer>()));

var app = builder.Build();

app.MapGraphQL();

app.Run();