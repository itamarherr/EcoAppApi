using DAL.Data;
using DAL.Models;
using EcoAppApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoAppApi.Controllers
{
    [Route ("api/[controller]")]
    [ApiController]
    [Authorize (Roles = "admin")] // Ensure only admin can access
    public class UsersController(
        UserManager<AppUser> _userManager,
        DALContext _context
        //SignInManager<AppUser> signInManager,
        //RoleManager<IdentityRole> roleManager
        ) : ControllerBase
    {


        [HttpGet ("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrEmpty (query))
            {
                return BadRequest ("Query Can't be empty");
            }
            var users = await _userManager.Users
                .Where (u => u.UserName != null && u.UserName.Contains (query) ||
                u.Email != null && u.Email.Contains (query) ||
                u.FirstName != null && u.FirstName.Contains (query) ||
                u.LastName != null && u.LastName.Contains (query))
                .ToListAsync ();
            var userDtos = users.Select (user => user.ToDto ()).ToList ();
            return Ok (userDtos);
        }
        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync ();
            var userDtos = users.Select (user => user.ToDto ()).ToList ();
            return Ok (userDtos);
        }

        // GET: api/Users/5
        [HttpGet ("{id}")]
        public async Task<ActionResult<AppUser>> GetAppUser(string id)
        {
            var appUser = await _context.Users.FindAsync (id);

            if (appUser == null)
            {
                return NotFound ();
            }

            return appUser;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut ("{id}")]
        public async Task<IActionResult> PutAppUser(string id, AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return BadRequest ();
            }

            _context.Entry (appUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync ();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppUserExists (id))
                {
                    return NotFound ();
                }
                else
                {
                    throw;
                }
            }

            return NoContent ();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AppUser>> PostAppUser(AppUser appUser)
        {
            _context.Users.Add (appUser);
            try
            {
                await _context.SaveChangesAsync ();
            }
            catch (DbUpdateException)
            {
                if (AppUserExists (appUser.Id))
                {
                    return Conflict ();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction ("GetAppUser", new { id = appUser.Id }, appUser);
        }

        // DELETE: api/Users/5
        [HttpDelete ("{id}")]
        public async Task<IActionResult> DeleteAppUser(string id)
        {
            var appUser = await _context.Users.FindAsync (id);
            if (appUser == null)
            {
                return NotFound ();
            }

            _context.Users.Remove (appUser);
            await _context.SaveChangesAsync ();

            return NoContent ();
        }

        private bool AppUserExists(string id)
        {
            return _context.Users.Any (e => e.Id == id);
        }
    }
}
