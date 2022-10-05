namespace API.Bag.Configuration;

public class GraphQLConfiguration
{
    public string? GatewayName { get; set; }
    public string? ServiceName { get; set; }
    public GraphQLRedisConfiguration? Redis { get; set; }
    public GraphQLStitchingConfiguration? Stitching { get; set; }
}

public class GraphQLRedisConfiguration
{
    public string? Endpoint { get; set; }
}

public class GraphQLStitchingConfiguration
{
    public bool Enabled { get; set; }
}
