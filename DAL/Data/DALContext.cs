﻿
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class DALContext(DbContextOptions<DALContext> options) :
    IdentityDbContext<AppUser, IdentityRole<string>, string> (options)
{
    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<Order> Orders { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating (modelBuilder);

        modelBuilder.Entity<Order> (entity =>
        {
            entity.HasOne (o => o.User)
           .WithMany (u => u.Orders)
           .HasForeignKey (o => o.UserId);

            entity.HasOne (o => o.Product)
            .WithMany (s => s.Orders)
            .HasForeignKey (o => o.ProductId);

            entity.Property (o => o.CreatedAt)
       .HasColumnName ("CreatedAt");

            entity.HasData (new[] {

              new Order
        {
            Id = 1,
            UserId = "1",
            ProductId = 1,
           DateForConsultancy = DateTime.Now,
            CreatedAt = DateTime.UtcNow,
            City = "SampleCity",
            Street = "SampleStreet",
            Number = 123
        },
        new Order
        {
            Id = 2,
            UserId = "1",
            ProductId = 1,
            DateForConsultancy = DateTime.Now,
            CreatedAt = DateTime.UtcNow,
            City = "AnotherCity",
            Street = "AnotherStreet",
            Number = 456
        }
             });
        });

        modelBuilder.Entity<Product> ()
            .HasData (new[] {
                new Product
                {
                        Id = 1,
                        Name = "Oak Consultancy",
                        Description = "Comprehensive assessment and consultation for oaks ",
                        Price = 1000.0M,

                },
            });

        var hasher = new PasswordHasher<AppUser> ();
        modelBuilder.Entity<IdentityRole<string>> ()
            .HasData (new[] {
                new IdentityRole   <string>()
                {
                    Id = "1",
                    Name = "admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            });

        modelBuilder.Entity<AppUser> ()
          .HasData (new[] {
              new AppUser()
              {
                  Id = "1",
                  Email = "itamarherr@gmail.com",
                  NormalizedEmail = "ITAMARHERR@GMAIL.COM",
                  UserName = "Itamar",
                  NormalizedUserName = "ITAMAR",
                  FirstName = "Itamar",
                  LastName = "Herr",
                  PhoneNumber = "1234567890",
                  ImageUrl  = "https://i.pravatar.cc/66",
                  SecurityStamp = Guid.NewGuid().ToString(),
                  PasswordHash = hasher.HashPassword(null, "ItamarherrPassword1!")
              }
          });
        modelBuilder.Entity<IdentityUserRole<string>> ().HasData (new[]
        {
             new IdentityUserRole<string>()
            {
                RoleId = "1",
                UserId = "1"
            }

        });

    }
}
