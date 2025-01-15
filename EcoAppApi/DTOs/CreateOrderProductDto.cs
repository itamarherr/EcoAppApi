using DAL.enums;
using System.ComponentModel.DataAnnotations;

namespace EcoAppApi.DTOs;

public class CreateOrderProductDto
{

    [Required]
    public string Name { get; set; } = "Oak Consultancy";
    public string Description { get; set; }

    public int NumberOfTrees { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string Street { get; set; }
    [Required]
    public int Number { get; set; }

    public int ConsultancyType { get; set; }
    public bool IsPrivateArea { get; set; }
    public DateTime? DateForConsultancy { get; set; }
    public bool Editing { get; set; }

    [Required]
    public int UserId { get; set; }
    [Required]
    public int ProductId { get; set; }
    public string Status { get; set; } = OrderStatus.pending.ToString ();
    public DateTime CreatedAt { get; set; }
    public string? AdditionalNotes { get; set; }
    public decimal? TotalPrice { get; set; }
}
