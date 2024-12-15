
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DAL.enums;

namespace DAL.Data;

public class DALContext(DbContextOptions<DALContext> options) :
    IdentityDbContext<AppUser, IdentityRole<int>, int>(options)
{
    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<Order> Orders { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.User)
           .WithMany(u => u.Orders)
           .HasForeignKey(o => o.UserId);

            entity.HasOne(o => o.Product)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.ProductId);

            //entity.Property(o => o.ImageUrl).IsRequired();

            entity.Property(o => o.CreatedAt)
       .HasColumnName("CreatedAt");

            entity.HasData(new[]
{
              new Order
        {
            Id = 1,
            UserId = 1,
            ProductId = 1,
           DateForConsultancy = DateTime.Now,
            CreatedAt = DateTime.UtcNow,
            Status = "Pending",
            //ImageUrl = "https://example.com/images/order1.png",
            City = "SampleCity",  
            Street = "SampleStreet",
            Number = 123
        },
        new Order
        {
            Id = 2,
            UserId = 1,
            ProductId = 1,
            DateForConsultancy = DateTime.Now,
            CreatedAt = DateTime.UtcNow,
            Status = "Completed",
            //ImageUrl = "https://example.com/images/order2.png",
            City = "AnotherCity", 
            Street = "AnotherStreet",
            Number = 456
        }
});

        });

        modelBuilder.Entity<Product>()
            .HasData(new[] {
                new Product
                {
                        Id = 1,
                        Name = "Oak Consultancy",
                        Description = "Comprehensive assessment and consultation for oaks ",
                        Price = 1000.0M,
                     
                },
            });

       
        var hasher = new PasswordHasher<AppUser>();
        modelBuilder.Entity<IdentityRole<int>>()
            .HasData(new[] {
                new IdentityRole<int>()
                {
                    Id = 1,
                    Name = "admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            });

        modelBuilder.Entity<AppUser>()
          .HasData(new[] {
              new AppUser()
              {
                  Id = 1,
                  Email = "itamarherr@gmail.com",
                  NormalizedEmail = "ITAMARHERR@GMAIL.COM",
                  UserName = "Itamar",
                  NormalizedUserName = "ITAMAR",
                  SecurityStamp = Guid.NewGuid().ToString(),
                  PasswordHash = hasher.HashPassword(null, "12345")
              }
          });
        modelBuilder.Entity<IdentityUserRole<int>>().HasData([
            new IdentityUserRole<int>()
            {
                RoleId = 1,
                UserId = 1
            }
        ]);
    }
}
