using DAL.Models;
using EcoAppApi.DTOs;
using EcoAppApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcoAppApi.Controllers;

[Route ("Api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    JwtUtils jwtService
    ) : ControllerBase
{
    [HttpPost ("Register")]
    public async Task<ActionResult> Register([FromForm] RegisterDto dto, IFormFile image)
    {
        foreach (var key in Request.Form.Keys)
        {
            Console.WriteLine ($"Form key: {key}, Value: {Request.Form[key]}");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest (ModelState);
        }
        var user = dto.ToUser ();

        Console.WriteLine ($"Generated User Id: {user.Id}"); // Ensure Id is not null
        var result = await userManager.CreateAsync (user, dto.Password);
        if (result.Succeeded)
        {
            if (image != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension (image.FileName).ToLower ();
                var maxFileSize = 5 * 1024 * 1024; // 5MB limit

                if (!allowedExtensions.Contains (extension))
                {
                    return BadRequest ("Invalid file type. Only JPG, PNG, and GIF files are allowed.");
                }

                if (image.Length > maxFileSize)
                {
                    return BadRequest ("File size exceeds the 5MB limit.");
                }

                // Ensure upload directory exists
                var uploadDir = Path.Combine (Directory.GetCurrentDirectory (), "wwwroot", "Uploads");
                if (!Directory.Exists (uploadDir))
                {
                    Directory.CreateDirectory (uploadDir);
                }

                // Use a unique file name to avoid overwriting existing files
                var uniqueFileName = $"{Guid.NewGuid ()}_{Path.GetFileName (image.FileName)}";
                var filePath = Path.Combine (uploadDir, uniqueFileName);

                using (var stream = new FileStream (filePath, FileMode.Create))
                {
                    await image.CopyToAsync (stream);
                }

                user.ImageUrl = $"/Uploads/{uniqueFileName}"; // Save the image path
                await userManager.UpdateAsync (user);
            }
            return Ok (new { message = "Registration successful.", imageUrl = user.ImageUrl });
        }
        return BadRequest (result.Errors);
    }

    [HttpPost ("Login")]
    public async Task<ActionResult> Login([FromBody] LoginDto dto)
    {

        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync (dto.Email);


            if (user is null || user.UserName is null)
            {
                return Unauthorized ();
            }


            var result = await signInManager.PasswordSignInAsync (
                user.UserName, dto.Password, false, false
            );

            if (result.Succeeded)
            {
                var roles = await userManager.GetRolesAsync (user);
                var role = roles.FirstOrDefault () ?? "user";

                Console.WriteLine ($"User {user.Email} has roles: {string.Join (", ", roles)}");

                var token = await jwtService.CreateToken (user);
                return Ok (new
                {
                    token,
                    role
                });
            }
            return Unauthorized ();
        }
        return BadRequest (ModelState);
    }
    // PUT: Api/Auth/Update
    [HttpPut ("update-my-profile")]
    public async Task<ActionResult> UpdateCurrentUserProfile([FromForm] UpdateUserDto dto, IFormFile image)
    {
        Console.WriteLine ("Received data:");
        Console.WriteLine ($"Email: {dto.Email}, Username: {dto.UserName}");
        Console.WriteLine ("Starting user update...");
        if (image != null)
        {
            Console.WriteLine ($"Received image: {image.FileName}");
        }
        else
        {
            Console.WriteLine ("No image received.");
        }
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
        var user = await userManager.FindByIdAsync (userIdClaim);
        if (user is null) return NotFound ("User not found");

        Console.WriteLine ($"User data before update: Email - {user.Email}, Username - {user.UserName}");

        user.Email = dto.Email ?? user.Email;
        user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
        user.FirstName = dto.FirstName ?? user.FirstName;
        user.UserName = dto.UserName ?? user.UserName;
        user.LastName = dto.LastName ?? user.LastName;

        if (image != null)
        {
            Console.WriteLine ($"Received image: {image.FileName}");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension (image.FileName).ToLower ();
            var maxFileSize = 5 * 1024 * 1024; // 5MB limit

            if (!allowedExtensions.Contains (extension))
            {
                return BadRequest ("Invalid file type. Only JPG, PNG, and GIF files are allowed.");
            }

            if (image.Length > maxFileSize)
            {
                return BadRequest ("File size exceeds the 5MB limit.");
            }

            var uploadDir = Path.Combine (Directory.GetCurrentDirectory (), "wwwroot", "Uploads");
            if (!Directory.Exists (uploadDir))
            {
                Directory.CreateDirectory (uploadDir);
            }

            var uniqueFileName = $"{Guid.NewGuid ()}_{Path.GetFileName (image.FileName)}";
            var filePath = Path.Combine (uploadDir, uniqueFileName);

            using (var stream = new FileStream (filePath, FileMode.Create))
            {
                await image.CopyToAsync (stream);
            }

            user.ImageUrl = $"/Uploads/{uniqueFileName}";
        }

        var result = await userManager.UpdateAsync (user);
        if (result.Succeeded)
            return Ok (new { message = "User updated successfully." });

        return BadRequest (result.Errors);
    }

    // DELETE:Api/Auth/Delete
    [HttpDelete ("Delete/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await userManager.FindByIdAsync (id.ToString ());
        if (user is null) return NotFound ("User not found");

        var result = await userManager.DeleteAsync (user);
        if (result.Succeeded)
        {
            return Ok (new { message = "User deleted successfull." });
        }
        return BadRequest (result.Errors);
    }


    //GET: Api/Auth/User/{id}
    [Authorize (Roles = "Admin")]
    [HttpGet ("User/{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await userManager.FindByIdAsync (id.ToString ());
        if (user is null) return NotFound ();
        return Ok (new { user.Id, user.Email, user.UserName, user.PhoneNumber });
    }

    // For security issued the access will be just will be for the admin.
    [Authorize (Roles = "Admin")]
    [HttpGet ("AllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userManager.Users.ToListAsync ();
        var userDto = users.Select (user => new
        {
            user.Id,
            user.Email,
            user.UserName,
            user.PhoneNumber
        }).ToList ();
        return Ok (userDto);
    }
    /*  [Authorize] */ // Ensures the user is authenticated


    [HttpGet ("my-profile")]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        var userIdClaim = User.Claims.FirstOrDefault (c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty (userIdClaim))
        {
            return Unauthorized (new { error = "User is not authenticated." });
        }

        var user = await userManager.FindByIdAsync (userIdClaim);
        if (user == null)
        {
            return NotFound (new { error = "User not found" });
        }


        return Ok (user.ToDto ());
    }
}
