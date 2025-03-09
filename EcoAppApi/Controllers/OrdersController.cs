using DAL.Data;
using DAL.Models;
using EcoAppApi.DTOs;
using EcoAppApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace EcoAppApi.Controllers;

[Authorize]
[Route ("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly DALContext _context;
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;
    private readonly PricingService _pricingService;
    private readonly JwtUtils _jwtService;
    private readonly IOrderRepository _orderRepository;

    public OrdersController(DALContext context, IOrderService orderService, PricingService pricingService, ILogger<OrdersController> logger, JwtUtils jwtService, IOrderRepository orderRepository)
    {
        _context = context;
        _orderService = orderService;
        _logger = logger;
        _pricingService = pricingService;
        _jwtService = jwtService;
        _orderRepository = orderRepository;
    }

    [Authorize (Roles = "admin")]
    [HttpGet ("search")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> SearchOrders([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace (query))
        {
            return BadRequest (new { message = "This Queiry is empty!" });
        }
        try
        {
            var orders = await _orderService.SearchOrderAsync (query);
            if (orders == null || !orders.Any ())
            {
                return NotFound (new { message = "No mathing orders found!" });
            }
            return Ok (orders);
        }
        catch (Exception ex)
        {

            _logger.LogError (ex, "Error searching orders");
            return StatusCode (500, new { message = "Internet server error", details = ex.Message });

        }

    }

    // GET: api/Orders
    [Authorize (Roles = "admin")]
    [HttpGet]
    public async Task<ActionResult> GetOrders(
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 20,
         [FromQuery] string sortBy = "CreatedAt",
         [FromQuery] bool descending = true,
         [FromQuery] string? userId = null,
         [FromQuery] string? userEmail = null
    )
    {
        try
        {
            var validSortProperties = new[] { "CreatedAt", "TotalPrice" };
            if (!validSortProperties.Contains (sortBy))
            {
                return BadRequest (new { message = "Invalid sortBy parameter." });
            }
            if (page <= 0 || pageSize <= 0)
                return BadRequest (new { message = "Page and pageSize must be greater than zero." });

            var (orderDtos, totalItems) = await _orderService.GetOrdersAsync (userId, userEmail, sortBy, descending, page, pageSize);

            return Ok (new { orders = orderDtos, totalItems });
        }
        catch (Exception ex)
        {
            _logger.LogError (ex, "Error fetching orders");
            return StatusCode (500, new { message = "Internal server error", details = ex.Message });
        }
    }

    //GET: api/Orders/5
    [Authorize (Roles = "admin")]
    [HttpGet ("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        if (id <= 0)
        {
            return BadRequest (new { message = "Invalid order ID." });
        }
        var order = await _context.Orders
             .Include (o => o.User)
             .FirstOrDefaultAsync (o => o.Id == id);

        if (order == null)
        {
            return NotFound ();
        }
        var response = new
        {
            order.Id,
            UserEmail = order.User != null ? order.User.Email : null,
            order.CreatedAt
        };
        return Ok (order.ToDto ());
    }


    [HttpGet ("my-orders")]
    public async Task<ActionResult<OrderDto>> GetCurrentUserOrders()
    {
        try
        {
            var userIdClaim = User.Claims.FirstOrDefault (c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty (userIdClaim))
            {
                return Unauthorized (new { error = "User is not authenticated." });
            }
            if (!Guid.TryParse (userIdClaim, out _))
            {
                return BadRequest (new { message = "Invalid user ID format." });
            }
            var order = await _orderService.GetMyOrderAsync (userIdClaim);
            if (order == null)
            {
                return NotFound (new { message = "No order found for the current user." });
            }
            return Ok (order);

        }
        catch (Exception ex)
        {
            _logger.LogError (ex, "Error fetching userws order");
            return StatusCode (500, new { message = "Intenal server error", details = ex.Message });
        }
    }


    [HttpGet ("my-orders/for-update")]
    public async Task<ActionResult<OrderDto>> GetLastOrdersForUpdates()
    {
        var userIdClaim = User.Claims.FirstOrDefault (c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty (userIdClaim))
        {
            return Unauthorized (new { error = "User is not authenticated." });
        }

        var orderDto = await _orderService.GetLastOrderForUpdateAsync (userIdClaim);
        if (orderDto == null)
        {
            return NotFound (new { error = "No orders found for the current user." });
        }


        return Ok (orderDto);
    }



    // PUT: api/Orders/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut ("my-orders/for-update")]
    public async Task<IActionResult> UpdateCurrentUserOrder([FromBody] UpdateOrderDto updateOrderDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany (v => v.Errors)
                .Select (e => e.ErrorMessage)
                .ToList ();

            return BadRequest (new { errors });
        }
        var userIdClaim = User.Claims.FirstOrDefault (c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty (userIdClaim))
        {
            return Unauthorized (new { error = "User is not authenticated." });
        }
        var success = await _orderService.UpdateCurrentUserOrderAsync (userIdClaim, updateOrderDto);
        if (!success)
        {
            return NotFound ("Order not found or you don't have access to update this order.");
        }

        return NoContent ();
    }

    // PUT: api/Orders/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [Authorize (Roles = "admin")]
    [HttpPut ("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        if (id <= 0 || !ModelState.IsValid)
        {
            return BadRequest ("Invalid input!");
        }

        var success = await _orderService.UpdateOrderAsync (id, updateOrderDto);
        if (!success)
        {
            return NotFound ("Order not found!");
        }

        return Ok (success);

    }

    // POST: api/Orders
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createDto)
    {

        var userIdClaim = User.Claims.FirstOrDefault (c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty (userIdClaim))
        {

            userIdClaim = User.Claims.FirstOrDefault (c =>
                c.Type == "sub" ||
                c.Type == "userId" ||
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            )?.Value;

            if (string.IsNullOrEmpty (userIdClaim))
            {
                return Unauthorized (new { error = "User is not authenticated.", details = "No valid user identifier found in claims" });
            }
        }
        if (!Guid.TryParse (userIdClaim, out _))
        {
            return BadRequest (new { error = "Invalid User ID format." });
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany (v => v.Errors)
                .Select (e => e.ErrorMessage)
                .ToList ();

            return BadRequest (new { errors });
        }

        try
        {
            var orderDto = await _orderService.CreateOrderAsync (createDto, userIdClaim);
            return CreatedAtAction (nameof (GetOrder), new { id = orderDto.Id }, orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError (ex, "Eror creatingorder");
            return StatusCode (500, new { message = "Internal server error", details = ex.Message });
        }
    }

    // DELETE: api/Orders/5
    [HttpDelete ("my-orders/latest")]
    public async Task<IActionResult> DeleteMyOrder()
    {
        var userIdClaim = User.Claims.FirstOrDefault (c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty (userIdClaim))
        {
            return Unauthorized ("You are not authorized.");
        }
        var success = await _orderService.DeleteLastOrderAsync (userIdClaim);

        if (!success)
        {
            return NotFound ("Order not found or you don't have access to delete this order.");
        }

        return NoContent ();

    }
    // DELETE:Api/Orders/Delete/{id}
    [Authorize (Roles = "admin")]
    [HttpDelete ("Delete/{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        if (id <= 0)
        {
            return BadRequest (new { message = "Invalid order id" });
        }
        try
        {
            await _orderRepository.DeleteOrderByIdAsync (id);
            return NoContent ();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound (new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode (500, new { message = "An error occurred.", details = ex.Message }); // Handle other errors
        }
    }
}