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

namespace EcoAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DALContext _context;

        public OrdersController(DALContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string sortBy = "CreateAt",
      [FromQuery] bool descending = true,
      [FromQuery] int? userId = null // Optional filter by user ID
  )
        {
            var query = _context.Orders
                .Include(o => o.User) // Include related User data
                .AsQueryable();

            // Apply filtering (optional)
            if (userId.HasValue)
            {
                query = query.Where(o => o.User.Id == userId.Value);
            }

            // Apply sorting
            query = descending
                ? query.OrderByDescending(o => EF.Property<object>(o, sortBy))
                : query.OrderBy(o => EF.Property<object>(o, sortBy));

            // Apply pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            // Execute query and return result
            var orders = await query.Select(o => new OrderDto
            {
                Id = o.Id,
               /* Name = o.Name,*/ // Assuming you want to include these fields
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt
            }).ToListAsync();

            return Ok(orders);
        }
        // GET: api/Orders/5
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

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createDto)
        {
            var user = await _context.Users.FindAsync(createDto.UserId);
            var service = await _context.Products.FindAsync(createDto.ProductId);

            if (user == null || service == null)
            {
                return BadRequest("Invalid User or Product ID.");
            }
            var order = new Order
            {
                UserId = createDto.UserId,
                ProductId = createDto.ProductId,
                AdditionalNotes = createDto.AdditionalNotes,
                TotalPrice = createDto.TotalPrice ?? service.Price,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
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
