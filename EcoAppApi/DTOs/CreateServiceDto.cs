using ApiExercise.DTOs;
using DAL.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace ApiExercise.DTOs
{
    public class CreateServiceDto
    {

        [Required]
        [MinLength(1), MaxLength(30)]
        public required string Name { get; set; }
        [Required]
        [MinLength(1), MaxLength(300)]
        public required bool Editing { get; set; }
       
        //public decimal Price { get; set; }
        //[Required]
        //[MinLength(1), MaxLength(30)]
        //public required string ImageUrl { get; set; }

        [Required]
        [Range(1, 100)]
        public required int CategoryId { get; set; }
    }
}
public static class CreateServiceDtoExtentions
{
    public static Service ToService(this CreateServiceDto dto)
    {
        return new Service
        {
            CategoryId = dto.CategoryId,
            //ImageUrl = dto.ImageUrl,
            Name = dto.Name,
            Editing = dto.Editing,
            //Price = dto.Price,
        };
    }

}