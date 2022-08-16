namespace GraphQL.ServiceB.Models
{
    public record class Comment(int id, int post_id, string name, string email, string body);
}
