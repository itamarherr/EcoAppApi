using Microsoft.AspNetCore.Identity;

namespace DAL.Models;

public class AppUser : IdentityUser<string>
{

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order> ();
    public AppUser()
    {
        Id = Guid.NewGuid ().ToString (); // Automatically assign a GUID to Id
    }
}
