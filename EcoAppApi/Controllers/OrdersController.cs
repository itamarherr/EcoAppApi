using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Models;
using EcoAppApi.DTOs;
using DAL.enums;
using Newtonsoft.Json;
using EcoAppApi.Utils;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using static NuGet.Packaging.PackagingConstants;


namespace EcoAppApi.Controllers
{
    [Route("api/[controller]")]
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

        // GET: api/Orders
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
                if (page <= 0 || pageSize <= 0)
                    return BadRequest(new { message = "Page and pageSize must be greater than zero." });
                var (orders, totalItems) = await _orderService.GetOrdersAsync(userId, userEmail, sortBy, descending, page, pageSize);

                return Ok(new { orders, totalItems });
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        //GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                 .Include(o => o.User)
                 .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }
            var response = new
            {
                order.Id,
                UserEmail = order.User != null ? order.User.Email : null,
                order.CreatedAt
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("my-orders")]
        public async Task<ActionResult<OrderDto>> GetCurrentUserOrders()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { error = "User is not authenticated." });
                }

                var order = await _orderService.GetMyOrderAsync(userIdClaim);
                if (order == null)
                {
                    return NotFound(new { message = "No order found for the current user." });
                }
                return Ok(order);

            } catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching userws order");
                return StatusCode(500, new { message = "Intenal server error" , details = ex.Message });    
            }
        }

        [Authorize]
        [HttpGet("my-orders/for-update")]
        public async Task<ActionResult<OrderDto>> GetLastOrdersForUpdates()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { error = "User is not authenticated." });
            }

            var lastOrder = await _context.Orders
                .Where(o => o.UserId == userIdClaim)
                 .Include(o => o.User)       // Ensure User is loaded
                 .Include(o => o.Product)
              .OrderByDescending(o => o.CreatedAt)
              .FirstOrDefaultAsync();
            if(lastOrder == null)
            {
                return NotFound(new { error = "No orders found for the current user."});
            }

            var userOrder = new OrderDto
            {
                Id = lastOrder.Id,
                UserId = userIdClaim,
                ProductId = lastOrder.ProductId,
                CreatedAt = lastOrder.CreatedAt,
                ServiceType = lastOrder.Product.Name ?? "Unknown Product",
                NumberOfTrees = lastOrder.NumberOfTrees,
                City = lastOrder.City,
                Street = lastOrder.Street,
                Number = lastOrder.Number,
                ConsultancyType = lastOrder.ConsultancyType,
                StatusType = lastOrder.StatusType,
                IsPrivateArea = lastOrder.IsPrivateArea,
                DateForConsultancy = lastOrder.DateForConsultancy,
                AdditionalNotes = lastOrder.AdditionalNotes,
                TotalPrice = lastOrder.TotalPrice,
                UserEmail = lastOrder.User.Email ?? "No Email",
                AdminNotes = lastOrder.AdditionalNotes
            };             
            return Ok(userOrder);
        }

        [HttpPut("my-orders/for-update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // No content on success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Invalid input or ModelState
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Order not found

        public async Task<IActionResult> UpdateCurrentUserOrder( [FromBody] UpdateOrderDto updateOrderDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { error = "User is not authenticated." });
            }

            // Find the order for the current user
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == updateOrderDto.Id && o.UserId == userIdClaim);

            if (order == null)
            {
                return NotFound("Order not found or you don't have access to update this order.");
            }
            try
            {
                order.AdminNotes = updateOrderDto.AdminNotes;
                order.TotalPrice = updateOrderDto.TotalPrice ?? order.TotalPrice;
                order.AdditionalNotes = updateOrderDto.AdditionalNotes ?? order.AdditionalNotes;
                order.NumberOfTrees = updateOrderDto.NumberOfTrees > 0 ? updateOrderDto.NumberOfTrees : order.NumberOfTrees;
                order.City = !string.IsNullOrEmpty(updateOrderDto.City) ? updateOrderDto.City : order.City;
                order.Street = !string.IsNullOrEmpty(updateOrderDto.Street) ? updateOrderDto.Street : order.Street;
                order.Number = updateOrderDto.Number > 0 ? updateOrderDto.Number : order.Number;
                order.ConsultancyType = updateOrderDto.ConsultancyType;
                order.IsPrivateArea = updateOrderDto.IsPrivateArea;
                order.DateForConsultancy = updateOrderDto.DateForConsultancy;
                order.StatusType = updateOrderDto.StatusType;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { error = "The order was updated by someone else. Please reload and try again." });
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, new { error = $"An unexpected error occurred: {ex.Message}" });
            }

            // Return success response
            return NoContent(); // 204 No Content indicates success with no body
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            if( id <= 0  || !ModelState.IsValid)
            {
                return BadRequest("Invalid input!");
            }
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id );
            if( order == null )
            {
                return BadRequest("No Order Found!");
            }

            order.AdminNotes = updateOrderDto.AdminNotes;
            order.TotalPrice = updateOrderDto.TotalPrice ?? order.TotalPrice;
            order.AdditionalNotes = updateOrderDto.AdditionalNotes ?? order.AdditionalNotes;
            order.NumberOfTrees = updateOrderDto.NumberOfTrees > 0 ? updateOrderDto.NumberOfTrees : order.NumberOfTrees;
            order.City = !string.IsNullOrEmpty(updateOrderDto.City) ? updateOrderDto.City : order.City;
            order.Street = !string.IsNullOrEmpty(updateOrderDto.Street) ? updateOrderDto.Street : order.Street;
            order.Number = updateOrderDto.Number > 0 ? updateOrderDto.Number : order.Number;
            order.ConsultancyType = updateOrderDto.ConsultancyType;
            order.IsPrivateArea = updateOrderDto.IsPrivateArea;
            order.DateForConsultancy = updateOrderDto.DateForConsultancy;
            order.StatusType = updateOrderDto.StatusType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(e => e.Id == id))
                {
                    return NotFound();
                    throw;
                }        
            }
            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createDto)
        {
           
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
         
                userIdClaim = User.Claims.FirstOrDefault(c =>
                    c.Type == "sub" ||
                    c.Type == "userId" ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                )?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { error = "User is not authenticated.", details = "No valid user identifier found in claims" });
                }
            }
            if (!Guid.TryParse(userIdClaim, out _))
            {
                return BadRequest(new { error = "Invalid User ID format." });
            }
     
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new  { errors });
            }

            try
            {
                var orderDto = await _orderService.CreateOrderAsync(createDto, userIdClaim);
                return CreatedAtAction(nameof(GetOrder), new { id = orderDto.Id }, orderDto);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Eror creatingorder");
                return StatusCode(500, new { message = "Internal server error", details=ex.Message});
            } 
        }

        // DELETE: api/Orders/5
        [HttpDelete("my-orders/latest")]
        public async Task<IActionResult> DeleteMyOrder()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("You are not authorized.");
            }
            var order = await _context.Orders
                .Where(o => o.UserId == userIdClaim)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
                
            if (order == null)
            {
                return NotFound("Order not found or you don't have access to delete this order.");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //private bool OrderExists(int id)
        //{
        //    return _context.Orders.Any(e => e.Id == id);
        //}
    }
}
