namespace API.Catalog.Models;

public record User(int Id, string name, int[]? FavoriteBrandIds = default);
