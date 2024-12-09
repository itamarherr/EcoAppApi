using DAL.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoAppApi.DTOs;

public class CreateOrderProductDto
{
    // Product properties
    [Required]
    public string Name { get; set; } = "Oak Consultancy";
    public string Description { get; set; }
 
    public int NumberOfTrees { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string Street { get; set; }
    [Required]
    public string Number { get; set; }
    public string ImageUrl { get; set; }
    public Purpose ConsultancyType { get; set; }
    public bool IsPrivateArea { get; set; }
    public DateTime DateForConsultancy { get; set; }
    public bool Editing { get; set; }

    // Order-related fields
    [Required]
    public int UserId { get; set; }
    [Required]
    public int ProductId { get; set; }
    public string? AdditionalNotes { get; set; }
    public decimal TotalPrice { get; set; }
}
