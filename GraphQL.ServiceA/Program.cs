using GraphQL.ServiceA.Configuration;
using GraphQL.ServiceA.Extensions;
using GraphQL.ServiceA.Types;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var graphQlConfig = new GraphQLConfiguration();
builder.Configuration.GetSection("GraphQL").Bind(graphQlConfig);

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect(graphQlConfig.Redis!.Endpoint))
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<QueryComment>()
    .InitializeOnStartup()
    .CustomPublishSchemaDefinition(graphQlConfig);

var app = builder.Build();

app.MapGraphQL();

app.Run();