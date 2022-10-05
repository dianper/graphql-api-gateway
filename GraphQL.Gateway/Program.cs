using GraphQL.Gateway.Configuration;
using GraphQL.Gateway.Extensions;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var graphQlConfig = new GraphQLConfiguration();
builder.Configuration.GetSection("GraphQL").Bind(graphQlConfig);

builder.Services
    .AddGraphQLServices(graphQlConfig);

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect(graphQlConfig.Redis!.Endpoint))
    .AddGraphQLServer()
    .AddRemoteSchemasFromRedis(graphQlConfig.ServiceName!, sp => sp.GetRequiredService<ConnectionMultiplexer>());

var app = builder.Build();

app.MapGraphQL();

app.Run();