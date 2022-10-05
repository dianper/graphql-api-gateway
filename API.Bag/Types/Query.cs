namespace API.Bag.Types;

using API.Bag.Models;
using API.Bag.Repositories;

public class Query
{
    public Bag? GetBagByUser(string username, [Service] BagRepository bagRepository) =>
        bagRepository.GetBagByUser(username);
}
