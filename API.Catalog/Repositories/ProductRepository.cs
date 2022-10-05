namespace API.Catalog.Repositories;

using API.Catalog.Models;

public class ProductRepository
{
    private readonly IEnumerable<Product> _products;

    public ProductRepository()
    {
        _products = new List<Product>
        {
            new (1, "T-Shit", "Black T-Shirt", 10, true, new (1, "Nike"), new (1, "Clothing")),
            new (2, "Shirt", "White Shirt", 15, true, new (2, "Oakley"), new (1, "Clothing")),
            new (3, "Skirt", "Pink Skirt", 12.5, true, new (3, "Levis"), new (1, "Clothing")),
            new (4, "Air Force", "Air Force 1'07", 50.9, true, new (1, "Nike"), new (2, "Shoes"))
        };
    }

    public IEnumerable<Product> GetProducts() => _products;

    public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);
}
