namespace API.Bag.Types;

using API.Bag.Models;
using API.Bag.Repositories;

public class Query
{
    public Bag? GetBagById(int id, [Service] BagRepository bagRepository) =>
        bagRepository.GetBagById(id);

    public Bag? GetBagByUsername(string username, [Service] BagRepository bagRepository) =>
        bagRepository.GetBagByUsername(username);
}
