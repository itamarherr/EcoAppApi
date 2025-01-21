using System.ComponentModel.DataAnnotations;

namespace EcoAppApi.DTOs;

public class UpdateUserDto
{
    [Required]
    public string Id { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [StringLength (50, MinimumLength = 3)]
    public string? UserName { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
}


