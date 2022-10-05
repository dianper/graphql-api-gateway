using GraphQL.ServiceB.Configuration;
using GraphQL.ServiceB.Extensions;
using GraphQL.ServiceB.Types;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var graphQlConfig = new GraphQLConfiguration();
builder.Configuration.GetSection("GraphQL").Bind(graphQlConfig);

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect("localhost"))
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .InitializeOnStartup()
    .CustomPublishSchemaDefinition(graphQlConfig);

var app = builder.Build();

app.MapGraphQL();

app.Run();