namespace API.Bag.Models;

public record Bag
{
    public int Id { get; init; }

    public IEnumerable<BagItem>? Items { get; init; }

    public User? User { get; init; }

    public double Total
    {
        get
        {
            double total = 0;

            foreach (var item in Items!)
            {
                total += item.Product.Price * item.Quantity;
            }

            return total;
        }
    }

    public Bag(int id, IEnumerable<BagItem>? items, User user)
    {
        this.Id = id;
        this.Items = items ?? new List<BagItem>();
        this.User = user;
    }
}
