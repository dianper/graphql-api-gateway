namespace API.Catalog.Models;

public record Product(
    int Id,
    string ShortDescription,
    string Description,
    double Price,
    bool HasStock,
    Brand Brand,
    Category Category);
