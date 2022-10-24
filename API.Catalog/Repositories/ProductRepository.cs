namespace API.Catalog.Repositories;

using API.Catalog.Models;

public class ProductRepository
{
    private readonly IEnumerable<Product> _products;

    public ProductRepository(BrandRepository brandRepository)
    {
        _products = Products(brandRepository);
    }

    public IEnumerable<Product> GetProducts() => _products;

    public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);

    private static IEnumerable<Product> Products(BrandRepository brandRepository)
    {
        var nike = brandRepository.GetByIndex(0);
        var oakley = brandRepository.GetByIndex(1);
        var levis = brandRepository.GetByIndex(2);
        var clothing = new Category(Id: 1, Description: "Clothing");
        var shoes = new Category(Id: 2, Description: "Shoes");

        return new List<Product>
        {
            new (
                Id: 1,
                ShortDescription: "T-Shit",
                Description: "Black T-Shirt",
                Price: 10,
                HasStock: true,
                Brand: nike,
                Category: clothing
            ),
            new (
                Id: 2,
                ShortDescription: "Shirt",
                Description: "White Shirt",
                Price: 15,
                HasStock: true,
                Brand: oakley,
                Category: clothing
            ),
            new (
                Id: 3,
                ShortDescription: "Pink",
                Description: "Pink Skirt",
                Price: 12.5,
                HasStock: true,
                Brand: levis,
                Category: clothing
            ),
            new (
                Id: 4,
                ShortDescription: "Air Force",
                Description: "Air Force 1'07",
                Price: 50.9,
                HasStock: true,
                Brand: nike,
                Category: shoes
            )
        };
    }
}
