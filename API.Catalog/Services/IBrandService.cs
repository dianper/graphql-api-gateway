namespace API.Catalog.Services;

using API.Catalog.Models;
using API.Catalog.Repositories;

public interface IBrandService
{
    Brand? GetBrandById(int id);

    IEnumerable<Brand> GetBrands();

    IEnumerable<Brand> GetBrandsByIds(int[] ids);

    IEnumerable<Brand> GetBrandsByUserId(int id);
}

public class BrandService : IBrandService
{
    private readonly BrandRepository brandRepository;
    private readonly UserRepository userRepository;

    public BrandService(
        BrandRepository brandRepository,
        UserRepository userRepository)
    {
        this.brandRepository = brandRepository;
        this.userRepository = userRepository;
    }

    public Brand? GetBrandById(int id)
    {
        return this.brandRepository.GetBrandById(id);
    }

    public IEnumerable<Brand> GetBrands()
    {
        return this.brandRepository.GetBrands();
    }

    public IEnumerable<Brand> GetBrandsByIds(int[] ids)
    {
        return this.brandRepository.GetBrandsByIds(ids);
    }

    public IEnumerable<Brand> GetBrandsByUserId(int id)
    {
        var brandIds = this.userRepository.GetUserById(id)?.FavoriteBrandIds;

        if (brandIds != null && brandIds.Any())
        {
            return this.brandRepository.GetBrandsByIds(brandIds);
        }

        return Enumerable.Empty<Brand>();
    }
}
