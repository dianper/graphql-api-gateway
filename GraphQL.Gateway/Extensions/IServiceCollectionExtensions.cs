using GraphQL.Gateway.Configuration;

namespace GraphQL.Gateway.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddGraphQLServices(this IServiceCollection services, GraphQLConfiguration graphQlConfiguration)
    {
        foreach (var item in graphQlConfiguration.Services!)
        {
            services.AddHttpClient(item.Name, c => c.BaseAddress = new Uri(item.Endpoint!));
        }

        return services;
    }
}
