using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace EcoAppApi.DTOs;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [MinLength (2), MaxLength (20)]
    public required string Username { get; set; }
    [Required]
    [DataType (DataType.Password)]
    public required string Password { get; set; }
}
public static class RegisterDtoExtentions
{
    public static AppUser ToUser(this RegisterDto dto)
    {
        return new AppUser ()
        {
            Id = Guid.NewGuid ().ToString (),
            Email = dto.Email,
            UserName = dto.Username,
            NormalizedEmail = dto.Email.ToUpper (),
            NormalizedUserName = dto.Username.ToUpper ()
        };
    }
}
