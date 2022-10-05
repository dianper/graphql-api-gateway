using API.Catalog.Configuration;
using API.Catalog.Extensions;
using API.Catalog.Repositories;
using API.Catalog.Types;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var graphQlConfig = new GraphQLConfiguration();
builder.Configuration.GetSection("GraphQL").Bind(graphQlConfig);

builder.Services
    .AddSingleton(ConnectionMultiplexer.Connect("localhost"))
    .AddSingleton<BrandRepository>()
    .AddSingleton<ProductRepository>()
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<QueryBrand>()
    .InitializeOnStartup()
    .CustomPublishSchemaDefinition(graphQlConfig);

var app = builder.Build();

app.MapGraphQL();

app.Run();