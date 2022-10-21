namespace API.Catalog.Repositories;

using API.Catalog.Models;

public class UserRepository
{
    private readonly IEnumerable<User> _users;

    public UserRepository()
    {
        _users = new List<User>
        {
            new (1, "user1", new[] { 1 }),
            new (2, "user2", new[] { 2, 3 }),
            new (3, "user3")
        };
    }

    public User? GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);
}
