using DAL.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Order
{
    [Key]
    [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required (ErrorMessage = "UserId is required.")]
    public string UserId { get; set; }
    public AppUser User { get; set; }
    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; }

    [Range (1, 30, ErrorMessage = "Number of trees must be at least 1. And no more then 30")]
    public int NumberOfTrees { get; set; }
    [Required (ErrorMessage = "City is required.")]
    [StringLength (50, ErrorMessage = "City name cannot exceed 50 characters.")]
    public string City { get; set; }
    [Required (ErrorMessage = "Street is required.")]
    [StringLength (50, ErrorMessage = "Street name cannot exceed 50 characters.")]
    public string Street { get; set; }
    [Range (1, int.MaxValue, ErrorMessage = "Street number must be positive.")]
    public int Number { get; set; }
    [Required (ErrorMessage = "ConsultancyType is required.")]
    public Purpose ConsultancyType { get; set; }
    public bool IsPrivateArea { get; set; }
    [Required (ErrorMessage = "Date for consultancy is required.")]
    public DateTime DateForConsultancy { get; set; }
    public string? AdditionalNotes { get; set; }
    public OrderStatus StatusType { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? AdminNotes { get; set; }
}
