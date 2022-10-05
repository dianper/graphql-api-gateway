namespace GraphQL.Gateway.Configuration
{
    public class GraphQLConfiguration
    {
        public string? ServiceName { get; set; }
        public IEnumerable<GraphQLServicesConfiguration>? Services { get; set; }
        public GraphQLRedisConfiguration? Redis { get; set; }
    }

    public class GraphQLServicesConfiguration
    {
        public string? Name { get; set; }
        public string? Endpoint { get; set; }
    }

    public class GraphQLRedisConfiguration
    {
        public string? Endpoint { get; set; }
    }
}
