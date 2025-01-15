using Microsoft.AspNetCore.Identity;

namespace DAL.Models;

public class AppUser : IdentityUser<string>
{
    public ICollection<Order> Orders { get; set; } = new List<Order> ();
    public AppUser()
    {
        Id = Guid.NewGuid ().ToString (); // Automatically assign a GUID to Id
    }
}
