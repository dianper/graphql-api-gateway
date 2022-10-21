namespace API.Bag.Repositories;

using API.Bag.Models;

public class BagRepository
{
    private readonly IList<Bag> _bags;

    public BagRepository()
    {
        _bags = new List<Bag>
        {
            new Bag(id: 1, userId: 1, items: new[] { new BagItem(ProductId: 1, Quantity: 1) }),
            new Bag(id: 2, userId: 1, items: new[] { new BagItem(ProductId: 2, Quantity: 5) }),
            new Bag(id: 3, userId: 2, items: new[] { new BagItem(ProductId: 3, Quantity: 10) }),
            new Bag(id: 4, userId: 3, items: new[] { new BagItem(ProductId: 4, Quantity: 15) }),
        };
    }

    public Bag? GetById(int id) => _bags.FirstOrDefault(b => b.Id == id);

    public Bag? AddToBag(int bagId, int productId, int quantity)
    {
        var bag = _bags.FirstOrDefault(b => b.Id == bagId);

        if (bag == null)
        {
            return default;
        }

        var bagItems = bag.Items.ToList();

        bagItems.Add(new BagItem(productId, quantity));

        bag.Items = bagItems;

        return bag;
    }
}
