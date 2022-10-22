namespace API.Catalog.Extensions;

using API.Catalog.Configuration;
using HotChocolate.Execution.Configuration;
using StackExchange.Redis;

public static class IRequestExecutionBuilderExtensions
{
    public static IRequestExecutorBuilder CustomPublishSchemaDefinition(
        this IRequestExecutorBuilder builder,
        GraphQLConfiguration graphQlConfiguration)
    {
        if (graphQlConfiguration.Stitching!.Enabled)
        {
            builder.PublishSchemaDefinition(c => c
                .SetName(graphQlConfiguration.ServiceName!)
                .AddTypeExtensionsFromFile("./Stitching.graphql")
                .PublishToRedis(graphQlConfiguration.GatewayName!, sp => sp.GetRequiredService<ConnectionMultiplexer>()));
        }

        return builder;
    }
}
