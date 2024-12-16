using Azure;
using DAL.enums;
using DAL.Models;
using Humanizer;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.ContentModel;

namespace EcoAppApi.DTOs

{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ServiceType { get; set; }
        public OrderStatus Status { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public int ConsultancyType { get; set; }
        public bool IsPrivateArea { get; set; }
        public DateTime DateForConsultancy { get; set; }
        public string? AdditionalNotes { get; set; }
        public string ImageUrl { get; set; }
        public decimal TotalPrice { get; set; }

        // Additional admin-specific information
        public string UserEmail { get; set; }
        public string AdminNotes { get; set; }
    }

    public static class OrderExtantion
    {
        public static OrderDto ToDto(this Order order)
        {

            return new OrderDto
            {
                Id = order.Id,
                UserName = order.User?.UserName ?? "Unknown", // Assuming AppUser has a UserName property
                City = order.City,
                Street = order.Street,
                Number = order.Number,
                ConsultancyType = (int)order.ConsultancyType,
                IsPrivateArea = order.IsPrivateArea,
                DateForConsultancy = order.DateForConsultancy,
                AdditionalNotes = order.AdditionalNotes,
                CreatedAt = order.CreatedAt,
                ServiceType = order.Product?.Name ?? "Unspecified",
                TotalPrice = order.TotalPrice,
                //ImageUrl = order.ImageUrl,
                UserEmail = order.User?.Email ?? "N/A", // Assuming AppUser has an Email property
                AdminNotes = order.AdditionalNotes,
                Status = Enum.TryParse(order.Status, true, out OrderStatus status)
                 ? status
                 : OrderStatus.pending
            };
        }
    }
}


