namespace API.Catalog.Types;

using API.Catalog.Models;
using API.Catalog.Services;

[ExtendObjectType(typeof(Query))]
public class QueryBrand
{
    public Brand? GetBrandById(int id, [Service] IBrandService service) =>
        service.GetBrandById(id);

    [UseFiltering]
    public IEnumerable<Brand> GetBrands([Service] IBrandService service) =>
        service.GetBrands();

    public IEnumerable<Brand> GetBrandsByIds(int[] ids, [Service] IBrandService service) =>
        service.GetBrandsByIds(ids);

    public IEnumerable<Brand> GetBrandsByUserId(int id, [Service] IBrandService service) =>
        service.GetBrandsByUserId(id);
}
