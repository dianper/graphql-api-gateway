using API.Gateway.Configuration;
using API.Gateway.Extensions;
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
//.RenameType("Product", "CatalogProduct", "catalog");

var app = builder.Build();

app.MapGraphQL();

app.Run();