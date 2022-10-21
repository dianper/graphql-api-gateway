namespace API.Bag.Models;

public record Bag
{
    public int Id { get; init; }

    public int UserId { get; set; }

    public IEnumerable<BagItem> Items { get; set; }

    public int Quantity { get => this.Items.Count(); }

    //public double Total
    //{
    //    get
    //    {
    //        double total = 0;

    //        foreach (var item in Items!)
    //        {
    //            total += item.Product.Price * item.Quantity;
    //        }

    //        return total;
    //    }
    //}

    public Bag(int id, int userId, IEnumerable<BagItem>? items)
    {
        this.Id = id;
        this.UserId = userId;
        this.Items = items ?? Enumerable.Empty<BagItem>();
    }
}
