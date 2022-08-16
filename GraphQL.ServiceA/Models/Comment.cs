namespace GraphQL.ServiceA.Models
{
    public record class Comment(int id, int post_id, string name, string email, string body);
}
