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

[ExtendObjectType(typeof(User))]
public class UserExtensions
{
    public IEnumerable<FavoriteBrand> GetFavoriteBrands([Parent] User user, [Service] BrandRepository brandRepository) =>
        brandRepository.GetByIds(user.BrandIds);
}
