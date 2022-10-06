namespace API.Bag.Repositories;

using API.Bag.Models;

public class BagRepository
{
    private readonly IList<Bag> _bags;
    private readonly IList<Product> _products;

    public BagRepository()
    {
        _products = new List<Product>
        {
            new (1, "Black T-Shirt", 10),
            new (2, "White Shirt", 15),
            new (3, "Pink Skirt", 12.5),
            new (4, "Nike Air Force 1", 50.9)
        };

        _bags = new List<Bag>
        {
            new Bag(1, new[] { new BagItem(_products[0], 1) }, new User(1, "user1")),
            new Bag(3, new[] { new BagItem(_products[1], 1) }, new User(2, "user2")),
            new Bag(4, new[] { new BagItem(_products[2], 1) }, new User(3, "user3")),
            new Bag(4, new[] { new BagItem(_products[3], 2) }, new User(4, "user4")),
        };
    }

    public Bag? GetBagById(int id) => _bags.FirstOrDefault(b => b.Id == id);

    public Bag? GetBagByUsername(string username) => _bags.FirstOrDefault(b => b.User?.Username == username);

    public Bag? AddToBag(int bagId, int productId, int quantity)
    {
        var bag = _bags.FirstOrDefault(b => b.Id == bagId);
        var product = _products.FirstOrDefault(p => p.Id == productId);

        if (bag == null || product == null) return default;

        var bagItems = bag.Items.ToList();

        bagItems.Add(new BagItem(product, quantity));

        bag.Items = bagItems;

        return bag;
    }
}
