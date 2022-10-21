﻿namespace API.Catalog.Repositories;

using API.Catalog.Models;

public class BrandRepository
{
    private readonly IEnumerable<Brand> _brands;

    public BrandRepository()
    {
        _brands = new List<Brand>
        {
            new (1, "Nike"),
            new (2, "Oakley"),
            new (3, "Levis")
        };
    }

    public IEnumerable<Brand> GetBrands() => _brands;

    public Brand? GetBrandById(int id) => _brands.FirstOrDefault(b => b.Id == id);

    public IEnumerable<Brand> GetBrandsByIds(int[] ids) => _brands.Where(b => ids.Contains(b.Id));
}
