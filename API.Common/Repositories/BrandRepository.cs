namespace API.Common.Repositories;

using API.Common.Models;

public class BrandRepository
{
    private readonly IEnumerable<FavoriteBrand> _brands;

    public BrandRepository()
    {
        _brands = new List<FavoriteBrand>
        {
            new (1, "Nike"),
            new (2, "Oakley"),
            new (3, "Levis")
        };
    }

    public IEnumerable<FavoriteBrand> GetByIds(int[]? ids) =>
        ids != null ? _brands.Where(b => ids.Contains(b.Id)) : Enumerable.Empty<FavoriteBrand>();
}
