using DAL.enums;
using System.ComponentModel.DataAnnotations;

namespace EcoAppApi.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public string? AdditionalNotes { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateOrderDto
    {
        public OrderStatus? Status { get; set; }
        public string? AdminNotes { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
