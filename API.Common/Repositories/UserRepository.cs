namespace API.Common.Repositories;

using API.Common.Models;

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

    public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

    public IEnumerable<User> GetAll() => _users;
}
