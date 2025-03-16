using DAL.Data;
using DAL.enums;
using DAL.Models;
using EcoAppApi.DTOs;

namespace EcoAppApi.Utils;

public class OrderService : IOrderService

{
    private readonly IOrderRepository _orderRepository;
    private readonly PricingService _pricingService;
    private readonly DALContext _context;

    public OrderService(IOrderRepository orderRepository, PricingService pricingService, DALContext context)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException (nameof (orderRepository));
        _pricingService = pricingService ?? throw new ArgumentNullException (nameof (pricingService));
        _context = context ?? throw new ArgumentNullException (nameof (context));
    }

    public async Task<IEnumerable<SearchOrderDto>> SearchOrderAsync(string query)
    {
        var orders = await _orderRepository.SearchOrdersAsync (query);
        return orders.Select (o => new SearchOrderDto
        {
            Id = o.Id,
            UserEmail = o.User.Email,
            UserName = o.User.UserName,
            ServiceType = o.Product != null ? o.Product.Name : "Unknown",
            City = o.City,
            StatusTypeString = Enum.GetName (typeof (OrderStatus), o.StatusType) ?? "Unknown",
            ConsultancyTypeString = Enum.GetName (typeof (Purpose), o.ConsultancyType) ?? "Unknown"
        }).ToList ();

    }

    public async Task<(List<OrderDto>, int)> GetOrdersAsync(string? userId, string? userEmail, string sortBy, bool descending, int page, int pageSize)
    {
        if (page <= 0 || pageSize <= 0)
        {
            throw new ArgumentException ("Page and pageSize must be greater than zero.");
        }
        var validSortProperties = new[] { "CreatedAt", "TotalPrice" };
        if (!validSortProperties.Contains (sortBy))
        {
            throw new ArgumentException ("Invalid sortBy parameter.");
        }
        if (_orderRepository == null)
        {
            throw new InvalidOperationException ("Order repository is not initialized.");
        }
        Console.WriteLine ($"userId: {userId}, userEmail: {userEmail}, sortBy: {sortBy}, descending: {descending}, page: {page}, pageSize: {pageSize}");

        var orders = await _orderRepository.GetPaginatedOrdersAsync (userId, userEmail, sortBy, descending, page, pageSize);

        if (orders == null)
        {
            throw new InvalidOperationException ("Failed to retrieve orders.");
        }
        var totalItems = await _orderRepository.GetTotalOrdersCountAsync (userId, userEmail);
        return (orders.Select (o => o.ToDto ()).ToList (), totalItems);


    }

    public async Task<OrderDto?> GetMyOrderAsync(string userId)
    {
        if (string.IsNullOrEmpty (userId))
        {
            throw new ArgumentException ("UserId is required.");
        }
        var order = await _orderRepository.GetLatestOrderAsync (userId);

        if (order == null)
        {
            throw new ArgumentException ("No orders found for the given user.");
        }

        return order.ToDto ();
    }
    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, string userId)
    {
        var totalPrice = _pricingService.CalculatePrice (
                 createDto.ConsultancyType,
                 createDto.NumberOfTrees,
                 createDto.IsPrivateArea
                 );
        var user = await _context.Users.FindAsync (userId);
        var product = await _context.Products.FindAsync (createDto.ProductId);

        if (user == null || product == null)
        {
            throw new ArgumentException ("Invalid UserId or ProductId");
        }

        var order = new Order
        {
            UserId = userId,
            ProductId = createDto.ProductId,
            AdditionalNotes = createDto.AdditionalNotes,
            TotalPrice = totalPrice,
            NumberOfTrees = createDto.NumberOfTrees,
            City = createDto.City,
            Street = createDto.Street,
            Number = createDto.Number,
            ConsultancyType = createDto.ConsultancyType,
            IsPrivateArea = createDto.IsPrivateArea,
            DateForConsultancy = createDto.DateForConsultancy,
            CreatedAt = DateTime.UtcNow,

            StatusType = createDto.StatusType,

        };
        var saveOrder = await _orderRepository.CreateOrderAsync (order);
        return saveOrder.ToDto ();
    }

    public async Task<OrderDto?> GetLastOrderForUpdateAsync(string userId)
    {
        var lastOrder = await _orderRepository.GetLatestOrderAsync (userId);
        return lastOrder?.ToDto ();
    }
    public async Task<OrderDto?> GetOrderForUpdateAsync(string userId)
    {
        var lastOrder = await _orderRepository.GetLatestOrderAsync (userId);
        return lastOrder?.ToDto ();
    }

    public async Task<bool> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
    {
        var order = await _orderRepository.GetOrderByIdAsync (id);
        if (order == null)
        {
            throw new ArgumentException ("Order not found");
        }
        if (string.IsNullOrEmpty (updateOrderDto.AdminNotes) &&
        updateOrderDto.DateForConsultancy <= DateTime.UtcNow)
        {
            throw new ArgumentException ("Invalid update data.");
        }

        ApplyUpdates (order, updateOrderDto);

        await _context.SaveChangesAsync ();
        return true;
    }

    public async Task<bool> UpdateCurrentUserOrderAsync(string userId, UpdateOrderDto updateOrderDto)
    {
        if (string.IsNullOrEmpty (userId))
        {
            throw new ArgumentException ("UserId is required.");
        }
        var order = await _orderRepository.GetLatestOrderAsync (userId);
        if (order == null)
        {
            throw new ArgumentException ("Order not found or user does not have permission.");
        }


        ApplyUpdates (order, updateOrderDto);

        await _context.SaveChangesAsync ();
        return true;
    }


    public async Task<bool> DeleteLastOrderAsync(string userId)
    {
        if (string.IsNullOrEmpty (userId))
        {
            throw new ArgumentException ("UserId is required.");
        }
        var order = await _orderRepository.GetLatestOrderAsync (userId);

        if (order == null)
        {
            throw new ArgumentException ("No orders found for the given user.");
        }

        await _orderRepository.DeleteMyOrderAsync (order);
        return true;
    }
    public async Task<bool> DeleteOrderByIdAsync(int orderId)
    {
        if (orderId <= 0)
        {
            throw new ArgumentException ("UserId is required.");
        }
        //var order = await _orderRepository.GetOrderByIdAsync(orderId);


        await _orderRepository.DeleteOrderByIdAsync (orderId);
        return true;
    }
    private void ApplyUpdates(Order order, UpdateOrderDto updateOrderDto)
    {
        // Update admin notes
        if (!string.IsNullOrEmpty (updateOrderDto.AdminNotes))
            order.AdminNotes = updateOrderDto.AdminNotes;

        // Update total price only if provided
        if (updateOrderDto.TotalPrice.HasValue)
            order.TotalPrice = updateOrderDto.TotalPrice.Value;

        // Update additional notes
        if (!string.IsNullOrEmpty (updateOrderDto.AdditionalNotes))
            order.AdditionalNotes = updateOrderDto.AdditionalNotes;

        // Update number of trees only if positive
        if (updateOrderDto.NumberOfTrees > 0)
            order.NumberOfTrees = updateOrderDto.NumberOfTrees;

        // Update city and street if not empty
        if (!string.IsNullOrEmpty (updateOrderDto.City))
            order.City = updateOrderDto.City;

        if (!string.IsNullOrEmpty (updateOrderDto.Street))
            order.Street = updateOrderDto.Street;

        // Update number only if positive
        if (updateOrderDto.Number > 0)
            order.Number = updateOrderDto.Number;

        // Update consultancy type and isPrivateArea (always overwrite)
        order.ConsultancyType = updateOrderDto.ConsultancyType;
        order.IsPrivateArea = updateOrderDto.IsPrivateArea;

        // Update date for consultancy (optional: validate if it's in the future)
        if (updateOrderDto.DateForConsultancy > DateTime.UtcNow)
            order.DateForConsultancy = updateOrderDto.DateForConsultancy;

        if (updateOrderDto.StatusType.HasValue)
            order.StatusType = updateOrderDto.StatusType.Value;
    }
}



