namespace API.Common.Types;

using API.Common.Models;
using API.Common.Repositories;

public class Query
{
    public User? GetUser(int id, [Service] UserRepository userRepository) =>
        userRepository.GetById(id);

    public IEnumerable<User> GetUsers([Service] UserRepository userRepository) =>
        userRepository.GetAll();
}
