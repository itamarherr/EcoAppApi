﻿using DAL.enums;
using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace EcoAppApi.DTOs
{
    public class UpdateOrderDto
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        //public string ImageUrl { get; set; }
        public string? AdminNotes { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? AdditionalNotes { get; set; }
        public int NumberOfTrees { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public Purpose ConsultancyType { get; set; }
        public bool IsPrivateArea { get; set; }
        [Required]
        public DateTime DateForConsultancy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserEmail { get; set; }
        public OrderStatus StatusType { get; set; } // The status of the order
        public string ServiceType { get; set; } // Name of the service/product
    }
}
