namespace API.Catalog.Repositories;

using API.Catalog.Models;

public class BrandRepository
{
    private readonly IEnumerable<Brand> _brands;

    public BrandRepository()
    {
        _brands = new List<Brand>
        {
            new (1, "Nike", "Nike"),
            new (2, "Oakley", "Oakley"),
            new (3, "Levis", "Levis")
        };
    }

    public IEnumerable<Brand> GetAll() => _brands;

    public Brand GetByIndex(int index) => _brands.ElementAt(index);

    public Brand? GetById(int id) => _brands.FirstOrDefault(b => b.Id == id);

    public IEnumerable<Brand> GetByIds(int[] ids) => _brands.Where(b => ids.Contains(b.Id));
}
