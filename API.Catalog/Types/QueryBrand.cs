namespace API.Catalog.Types;

using API.Catalog.Models;
using API.Catalog.Repositories;

[ExtendObjectType(typeof(Query))]
public class QueryBrand
{
    public IEnumerable<Brand?> GetBrands([Service] BrandRepository brandRepository) =>
        brandRepository.GetBrands();
}
