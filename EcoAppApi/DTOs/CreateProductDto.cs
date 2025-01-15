using DAL.Models;
using EcoAppApi.DTOs;
using System.ComponentModel.DataAnnotations;

namespace EcoAppApi.DTOs
{
    public class CreateProductDto
    {

        [Required]
        [MinLength (1), MaxLength (30)]
        public required string Name { get; set; }
        [Required]
        public required bool Editing { get; set; }
        public decimal Price { get; set; }

    }
}
public static class CreateProductDtoExtentions
{
    public static Product ToProduct(this CreateProductDto dto)
    {
        return new Product
        {

            Name = dto.Name,
            Editing = dto.Editing,
            Price = dto.Price,
        };
    }
}