namespace API.Bag.Repositories;

using API.Bag.Models;

public class BagRepository
{
    private readonly IList<Bag> _bag;

    public BagRepository()
    {
        _bag = new List<Bag>
        {
            new Bag(1, new[] { new BagItem(new Product(1, "Black T-Shirt", 10), 1) }, new User(1, "user1")),
            new Bag(3, new[] { new BagItem(new Product(2, "White Shirt", 15), 1) }, new User(2, "user2")),
            new Bag(4, new[] { new BagItem(new Product(3, "Pink Skirt", 12.5), 1) }, new User(3, "user3")),
            new Bag(4, new[] { new BagItem(new Product(4, "Nike Air Force 1", 50.9), 2) }, new User(4, "user4")),
        };
    }

    public Bag? GetBagByUser(string username) => _bag.FirstOrDefault(b => b.User?.Username == username);
}
