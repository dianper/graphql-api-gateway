namespace API.Bag.Types;

using API.Bag.Models;
using API.Bag.Repositories;

public class Mutation
{
    public Bag? AddToBag(int id, int productId, int quantity, [Service] BagRepository bagRepository) =>
        bagRepository.AddToBag(id, productId, quantity);
}
