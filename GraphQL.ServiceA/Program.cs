using GraphQL.ServiceA.Types;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect("localhost"))
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<QueryComment>()
    .InitializeOnStartup()
    .PublishSchemaDefinition(c => c
        .SetName("serviceA")
        .PublishToRedis("gateway", sp => sp.GetRequiredService<ConnectionMultiplexer>()));

var app = builder.Build();

app.MapGraphQL();

app.Run();