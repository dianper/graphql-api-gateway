using API.Bag.Configuration;
using API.Bag.Extensions;
using API.Bag.Repositories;
using API.Bag.Types;
using Framework.Diagnostics.ExecutionEvents;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var graphQlConfig = new GraphQLConfiguration();
builder.Configuration.GetSection("GraphQL").Bind(graphQlConfig);

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect(graphQlConfig.Redis!.Endpoint))
    .AddSingleton<BagRepository>()
    .AddGraphQLServer()
    .AddDiagnosticEventListener<CustomExecutionEventListener>()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddMutationConventions(applyToAllMutations: true)
    .InitializeOnStartup()
    .CustomPublishSchemaDefinition(graphQlConfig);

var app = builder.Build();

app.MapGraphQL();

app.Run();