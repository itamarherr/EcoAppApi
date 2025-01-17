﻿using DAL.Models;

namespace DAL.Data;

public interface IOrderRepository
{
    Task<List<Order>> GetPaginatedOrdersAsync(string? userId, string? userEmail, string sortBy, bool descending, int page, int pageSize);
    Task<int> GetTotalOrdersCountAsync(string? userId, string? userEmail);
    Task<Order?> GetLatestOrderAsync(string userId);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> GetOrderByIdAsync(int id);
    Task DeleteOrderAsync(Order order);

}
