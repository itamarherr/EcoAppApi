using DAL.Models;

namespace EcoAppApi.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required bool Editing { get; set; }
    public decimal Price { get; set; }  
}
public static class ProductExtensions
{
    public static ProductDto ToDto(this Product p)
    {
        return new ProductDto()
        {
            Id = p.Id,
            Name = p.Name,
            Editing = p.Editing,
            Price = p.Price,
        };
    }
}
