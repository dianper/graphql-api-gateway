namespace API.Bag.Models;

public record Bag
{
    public int Id { get; init; }

    public int UserId { get; set; }

    public IEnumerable<BagItem> Items { get; set; }

    public int Count { get => this.Items.Count(); }

    public Bag(int id, int userId, IEnumerable<BagItem>? items)
    {
        this.Id = id;
        this.UserId = userId;
        this.Items = items ?? Enumerable.Empty<BagItem>();
    }
}
