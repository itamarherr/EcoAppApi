using DAL.enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; }
    public AppUser User { get; set; }
    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; }


    // Order-specific fields
    public int NumberOfTrees { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public int Number { get; set; }
    public Purpose ConsultancyType { get; set; }
    public bool IsPrivateArea { get; set; }

    [Required]
    public DateTime DateForConsultancy { get; set; }
    public string? AdditionalNotes { get; set; }
    //public string ImageUrl { get; set; }
    //public DateTime OrderDate { get; set; } = DateTime.Now;
    public OrderStatus StatusType { get; set; }

    //// Pricing and metadata
    //[Column(TypeName = "Money")]
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    //public DateTime LastUpdate { get; set; } = DateTime.Now;

    public string? AdminNotes { get; set; }
}
