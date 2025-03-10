﻿using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace EcoAppApi.Utils;

public class JwtUtils(IConfiguration configuration, UserManager<AppUser> userManager)
{

    public async Task<string> CreateToken(AppUser user)
    {
        Console.WriteLine ("Hello");
        var jwtSettings = configuration.GetSection ("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new Exception ("Secret key must be set in app settings");

        var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (secretKey));



        //JWT is a collection of claims
        var claims = new List<Claim> ()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email), // Include email for debugging
        };
        Console.WriteLine ($"Creating token for user ID: {user.Id}");
        foreach (var claim in claims)
        {
            Console.WriteLine ($"{claim.Type}: {claim.Value}");
        }
        var roles = await userManager.GetRolesAsync (user);
        Console.WriteLine ($"User roles: {string.Join (", ", roles)}");

        foreach (var role in roles)
        {
            claims.Add (new Claim (ClaimTypes.Role, role));
        }


        var isAdmin = await userManager.IsInRoleAsync (user, "admin");

        if (isAdmin)
        {
            claims.Add (new Claim (ClaimTypes.Role, "admin"));
        }
        //That is Encrypted using a SECRET key:

        var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken (
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            expires: DateTime.UtcNow.AddDays (1),
            claims: claims,
            signingCredentials: creds
        );

        //convert the token to a string:
        var jwt = new JwtSecurityTokenHandler ().WriteToken (token);


        return jwt;
    }
}
