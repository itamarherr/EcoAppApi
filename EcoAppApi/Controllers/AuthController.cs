using EcoAppApi.DTOs;
using EcoAppApi.Utils;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcoAppApi.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<AppUser> UserManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole<int>>roleManager,
        JwtUtils jwtService
        ) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterDto dto)
        {
            if(ModelState.IsValid)
            {
                var result = await UserManager.CreateAsync(dto.ToUser(), dto.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {

            if (ModelState.IsValid)
            {
                var user =  await UserManager.FindByEmailAsync(dto.Email);

                //check that email exists
                if (user is null || user.UserName is null)
                {
                    return Unauthorized();
                }

                //verify password
                var result = await signInManager.PasswordSignInAsync(
                    user.UserName, dto.Password, false, false
                );

                if (result.Succeeded)
                {
                    var token = await jwtService.CreateToken(user);
                    return Ok(new { token }); //TODO replace with jwt 
                }
                return Unauthorized();
            }
            return BadRequest(ModelState);
        }
    }
}
