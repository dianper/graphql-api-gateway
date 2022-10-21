namespace API.Bag.Types;

using API.Bag.Models;
using API.Bag.Repositories;

public class Query
{
    public Bag? GetBag(int id, [Service] BagRepository bagRepository) =>
        bagRepository.GetById(id);
}
