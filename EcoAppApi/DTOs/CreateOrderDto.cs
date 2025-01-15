using DAL.enums;
using EcoAppApi.Attributrs;
using System.ComponentModel.DataAnnotations;

namespace EcoAppApi.DTOs
{
    public class CreateOrderDto
    {
        [Required (ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }
        [Required (ErrorMessage = "ProductId is required.")]
        [Range (1, int.MaxValue, ErrorMessage = " ProductId Must be a positive number.")]
        public int ProductId { get; set; }
        public string? AdminNotes { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? AdditionalNotes { get; set; }
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
        [FutureDate (ErrorMessage = "Date for consultancy must be in the future.")]
        public DateTime DateForConsultancy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserEmail { get; set; }
        public OrderStatus StatusType { get; set; } // The status of the order
        public string ServiceType { get; set; } // Name of the service/product
    }


}
