using DAL.enums;
using DAL.Models;

namespace EcoAppApi.DTOs;
public class OrderDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public string UserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ServiceType { get; set; }
    public OrderStatus StatusType { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public int Number { get; set; }
    public Purpose ConsultancyType { get; set; }
    public int NumberOfTrees { get; set; }
    public bool IsPrivateArea { get; set; }
    public DateTime DateForConsultancy { get; set; }
    public string? AdditionalNotes { get; set; }

    public decimal TotalPrice { get; set; }

    public string UserEmail { get; set; }
    public string AdminNotes { get; set; }
    public string ConsultancyTypeString => Enum.GetName (typeof (Purpose), ConsultancyType) ?? "Unknown";
    public string StatusTypeString => Enum.GetName (typeof (OrderStatus), StatusType) ?? "Pending";
}

public static class OrderExtantion
{
    public static OrderDto ToDto(this Order order)
    {

        return new OrderDto
        {
            Id = order.Id,

            UserId = order.UserId,
            ProductId = order.ProductId,
            UserName = order.User?.UserName ?? "Unknown",
            City = order.City,
            Street = order.Street,
            Number = order.Number,
            ConsultancyType = order.ConsultancyType,
            IsPrivateArea = order.IsPrivateArea,
            DateForConsultancy = order.DateForConsultancy,
            NumberOfTrees = order.NumberOfTrees,
            AdditionalNotes = order.AdditionalNotes,
            CreatedAt = order.CreatedAt,
            ServiceType = order.Product?.Name ?? "Unspecified",
            TotalPrice = order.TotalPrice,
            UserEmail = order.User?.Email ?? "N/A",
            AdminNotes = order.AdditionalNotes,
            StatusType = order.StatusType

        };
    }
}


