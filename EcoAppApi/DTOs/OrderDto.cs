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
                CreatedAt = order.OrderDate,
                ServiceType = order.Service?.Name ?? "Unspecified", // Assuming Service has a Name property
                Status = Enum.TryParse(order.Status, out OrderStatus status) ? status : OrderStatus.pending,
                TotalPrice = order.TotalPrice,
                UserEmail = order.User?.Email ?? "N/A", // Assuming AppUser has an Email property
                AdminNotes = order.additionalNotes
            };
        }
    }
}


