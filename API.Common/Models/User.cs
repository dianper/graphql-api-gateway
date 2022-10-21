namespace API.Common.Models;

public record User(int Id, string Username, int[]? BrandIds = default);