using Microsoft.AspNetCore.Identity;

namespace DAL.Models;

public class AppUser : IdentityUser<int>
{
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
