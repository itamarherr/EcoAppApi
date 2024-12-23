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

namespace EcoAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DALContext _context;
        //private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(DALContext context, /*IOrderService orderService,*/ ILogger<OrdersController> logger)
        {
            _context = context;
            //_orderService = orderService;
            _logger = logger;
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
     
            
            var userOrders = await _context.Orders
                .Where(o => o.UserId == userIdClaim)
              .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = userIdClaim,
                    UserName = o.User.UserName,
                    CreatedAt = o.CreatedAt,
                    ServiceType = o.Product.Name,  // Assuming this comes from Product
          
                    City = o.City,
                    Street = o.Street,
                    Number = o.Number,
                    ConsultancyType = o.ConsultancyType,
                    StatusType = o.StatusType,
                    IsPrivateArea = o.IsPrivateArea,
                    DateForConsultancy = o.DateForConsultancy,
                    AdditionalNotes = o.AdditionalNotes,

                    TotalPrice = o.TotalPrice,
                    UserEmail = o.User.Email,
                    //AdminNotes = o.AdminNotes
                })
                .FirstOrDefaultAsync();

            return Ok(userOrders);
 
        }
        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto orderDto)
        //{
        //    try
        //    {
        //        var updatedOrder = await _orderService.UpdateOrderAsync(id, orderDto);
        //        return Ok(updatedOrder);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //}

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { error = "User is not authenticated." });
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

            var order = new Order
            {
                UserId = userIdClaim,
                ProductId = createDto.ProductId,
                //ImageUrl = createDto.ImageUrl,
                AdditionalNotes = createDto.AdditionalNotes,  
                //TotalPrice = createDto.TotalPrice ?? service.Price,
                NumberOfTrees = createDto.NumberOfTrees,
                City = createDto.City,
                Street = createDto.Street,
                Number = createDto.Number,
                ConsultancyType = (Purpose)createDto.ConsultancyType,
                IsPrivateArea = createDto.IsPrivateArea,
                DateForConsultancy = createDto.DateForConsultancy,
                CreatedAt = DateTime.UtcNow,
                //Product = service,
                //User = user,
                StatusType = createDto.Status,

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
