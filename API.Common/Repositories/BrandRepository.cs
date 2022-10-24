namespace API.Common.Repositories;

using API.Common.Models;

public class BrandRepository
{
    private readonly IEnumerable<CommonBrand> _commonBrands;
    private readonly IEnumerable<Brand> _brands;

    public BrandRepository()
    {
        _commonBrands = new List<CommonBrand>
        {
            new (1, "Nike"),
            new (2, "Oakley"),
            new (3, "Levis")
        };

        _brands = new List<Brand>
        {
            new (1, "Nike"),
            new (2, "Oakley"),
            new (3, "Levis")
        };
    }

    public IEnumerable<CommonBrand> GetByIds(int[]? ids) =>
        ids != null ? _commonBrands.Where(b => ids.Contains(b.Id)) : Enumerable.Empty<CommonBrand>();

    public IEnumerable<Brand> GetBrandsByIds(int[]? ids) =>
        ids != null ? _brands.Where(b => ids.Contains(b.Id)) : Enumerable.Empty<Brand>();
}
