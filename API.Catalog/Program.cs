using API.Catalog.Configuration;
using API.Catalog.Extensions;
using API.Catalog.Repositories;
using API.Catalog.Services;
using API.Catalog.Types;
using Framework.Diagnostics.ExecutionEvents;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var graphQlConfig = new GraphQLConfiguration();
builder.Configuration.GetSection("GraphQL").Bind(graphQlConfig);

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect(graphQlConfig.Redis!.Endpoint))
    .AddSingleton<BrandRepository>()
    .AddSingleton<ProductRepository>()
    .AddSingleton<UserRepository>()
    .AddScoped<IBrandService, BrandService>()
    .AddGraphQLServer()
    .AddDiagnosticEventListener<CustomExecutionEventListener>()
    .AddFiltering()
    .AddQueryType<Query>()
    .AddTypeExtension<QueryBrand>()
    .InitializeOnStartup()
    .CustomPublishSchemaDefinition(graphQlConfig);

var app = builder.Build();

app.MapGraphQL();

app.Run();