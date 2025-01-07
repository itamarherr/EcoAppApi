﻿using System;
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
        //private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        private readonly PricingService _pricingService;
        private readonly JwtUtils _jwtService;

        public OrdersController(DALContext context, PricingService pricingService, ILogger<OrdersController> logger, JwtUtils jwtService)
        {
            _context = context;
            //_orderService = orderService;
            _logger = logger;
            _pricingService = pricingService;
            _jwtService = jwtService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult> GetOrders(
             [FromQuery] int page = 1,
             [FromQuery] int pageSize = 20,
             [FromQuery] string sortBy = "CreatedAt",
             [FromQuery] bool descending = true,
             [FromQuery] string? userId = null,
             [FromQuery] string userEmail = null
        )
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                    return BadRequest(new { message = "Page and pageSize must be greater than zero." });
               
                var validSortFields = new[] { "CreatedAt", "TotalPrice", "Id" };
                if (!validSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
                    return BadRequest(new { message = $"Invalid sort field: {sortBy}" });


                var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .AsQueryable();

                if (!string.IsNullOrEmpty(userId))
                    query = query.Where(o => o.User.Id == userId);

                if (!string.IsNullOrEmpty(userEmail))
                    query = query.Where(o => o.User.Email.Contains(userEmail));

                var totalItems = await query.CountAsync();

                query = descending
               ? query.OrderByDescending(o => EF.Property<object>(o, sortBy))
               : query.OrderBy(o => EF.Property<object>(o, sortBy));
               
                var orders = await query
                      .Skip((page - 1) * pageSize)
                      .Take(pageSize)
                      .Select(o => new OrderDto
            {
                    Id = o.Id,
                    UserEmail = o.User != null ? o.User.Email : null,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt
                }).ToListAsync();

                return Ok( orders );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch orders with params: page={Page}, pageSize={PageSize}, sortBy={SortBy}, descending={Descending}",
                    page, pageSize, sortBy, descending, userId, userEmail);
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
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    
            if (string.IsNullOrEmpty(userIdClaim) )
            {
                return Unauthorized(new { error = "User is not authenticated." });
            }


            var orders = await _context.Orders
                .Where(o => o.UserId == userIdClaim)
                 .Include(o => o.User)       // Ensure User is loaded
                 .Include(o => o.Product)
              .OrderByDescending(o => o.CreatedAt)
              .ToListAsync();

            foreach (var order in orders)
            {
                Console.WriteLine(value: $"Order ID: {order.Id}, User: {order.User?.UserName ?? "NULL"}, Product: {order.Product?.Name ?? "NULL"}, NumberOfTrees: {order.NumberOfTrees}");
            }
            var userOrders = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = userIdClaim,
                    UserName = o.User.UserName ?? "Unknown User",
                    CreatedAt = o.CreatedAt,
                    ServiceType = o.Product.Name ?? "Unknown Product", 
                    NumberOfTrees = o.NumberOfTrees,
                    City = o.City,
                    Street = o.Street,
                    Number = o.Number,
                    ConsultancyType = o.ConsultancyType,
                    StatusType = o.StatusType,
                    IsPrivateArea = o.IsPrivateArea,
                    DateForConsultancy = o.DateForConsultancy,
                    AdditionalNotes = o.AdditionalNotes,
                    TotalPrice = o.TotalPrice,
                    UserEmail = o.User.Email ?? "No Email",
                    //AdminNotes = o.AdminNotes

                })
                .FirstOrDefault();

            return Ok(userOrders);
 
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
        //[ProducesResponseType(StatusCodes.Status204NoContent)] // No content on success
        //[ProducesResponseType(StatusCodes.Status400BadRequest)] // Invalid input or ModelState
        //[ProducesResponseType(StatusCodes.Status404NotFound)] // Order not found

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

            //    try
            //    {
            //        var updatedOrder = await _orderService.UpdateOrderAsync(id, orderDto);
            //        return Ok(updatedOrder);
            //    }
            //    catch (KeyNotFoundException ex)
            //    {
            //        return NotFound(ex.Message);
            //    }
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createDto)
        {
            //foreach (var claim in User.Claims)
            //{
            //    Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            //}
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            //var backupUserIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

      

            if (string.IsNullOrEmpty(userIdClaim))
            {
                // Try alternative claim types
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
            //var user = await _context.Users.FindAsync(createDto.UserId);
            //var service = await _context.Products.FindAsync(createDto.ProductId);

            //if (user == null || service == null)
            //{
            //    return BadRequest(new { errors = new[] { "Invalid User or Product ID." } });
            //}
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new  { errors });
            }
            //if (!Enum.IsDefined(typeof(Purpose), createDto.ConsultancyType))
            //{
            //    return BadRequest("Invalid Consultancy Type.");
            //}
            var totalPrice = _pricingService.CalculatePrice(
                createDto.ConsultancyType,
                createDto.NumberOfTrees,
                createDto.IsPrivateArea
                );

            var order = new Order
            {
                UserId = userIdClaim,
                ProductId = createDto.ProductId,
                //ImageUrl = createDto.ImageUrl,
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
                //Product = service,
                //User = user,
                StatusType = createDto.StatusType,

                //LastUpdate = DateTime.UtcNow
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order.ToDto());
        }


        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
