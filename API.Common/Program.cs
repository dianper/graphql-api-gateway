using API.Common.Configuration;
using API.Common.Extensions;
using API.Common.Repositories;
using API.Common.Types;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var graphQlConfig = new GraphQLConfiguration();
builder.Configuration.GetSection("GraphQL").Bind(graphQlConfig);

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect(graphQlConfig.Redis!.Endpoint))
    .AddSingleton<UserRepository>()
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .InitializeOnStartup()
    .CustomPublishSchemaDefinition(graphQlConfig);

var app = builder.Build();

app.MapGraphQL();

app.Run();
