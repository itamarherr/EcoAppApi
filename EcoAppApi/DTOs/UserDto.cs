using DAL.Models;

namespace EcoAppApi.DTOs;


public class UserDto
{

    public string Id { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string PhoneNumber { get; set; }
    public string? ImageUrl { get; set; }
}
public static class UserExtensions
{
    public static UserDto ToDto(this AppUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            UserEmail = user.Email,
            PhoneNumber = user.PhoneNumber,
            ImageUrl = user.ImageUrl,
        };
    }
}
