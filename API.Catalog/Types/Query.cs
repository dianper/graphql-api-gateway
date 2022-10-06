namespace API.Catalog.Types;

using API.Catalog.Models;
using API.Catalog.Repositories;

public class Query
{
    public IEnumerable<Product>? GetProducts([Service] ProductRepository productRepository) =>
        productRepository.GetProducts();

    public Product? GetProductById(int id, [Service] ProductRepository productRepository) =>
        productRepository.GetProductById(id);
}
