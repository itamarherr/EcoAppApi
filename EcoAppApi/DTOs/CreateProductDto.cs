using ApiExercise.DTOs;
using DAL.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace ApiExercise.DTOs
{
    public class CreateProductDto
    {

        [Required]
        [MinLength(1), MaxLength(30)]
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
            //ImageUrl = dto.ImageUrl,
            Name = dto.Name,
            Editing = dto.Editing,
            Price = dto.Price,
        };
    }
}