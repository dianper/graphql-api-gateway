namespace GraphQL.ServiceA.Types
{
    [ExtendObjectType(typeof(Query))]
    public class QueryComment
    {
        public string CustomComment() => "Custom Comment";
    }
}
