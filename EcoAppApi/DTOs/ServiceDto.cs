using DAL.Models;

namespace ApiExercise.DTOs;


public class ServiceDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required bool Editing { get; set; }
    //public decimal Price { get; set; }
    //public required string ImageUrl { get; set; }
    public required string Category { get; set; }
}

public static class ServiceExtensions
{
    public static ServiceDto ToDto(this Service s)
    {
        return new ServiceDto()
        {
            Category = s.Category.Name,
            Id = s.Id,
            Name = s.Name,
           Editing = s.Editing,
            //Price = s.Price,
            //ImageUrl = s.ImageUrl,
        };
    }
}

//p.ToDto()