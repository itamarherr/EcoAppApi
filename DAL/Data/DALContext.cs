
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DAL.Data;

public class DALContext(DbContextOptions<DALContext> options) : 
    IdentityDbContext<AppUser, IdentityRole<int>, int>(options)
{
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Service> Services { get; set; } = default!;

    public DbSet<CheckListItem> CheckListItems { get; set; }
    public DbSet<Order> Orders { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
           .HasOne(o => o.User)
           .WithMany(u => u.Orders)
           .HasForeignKey(o => o.UserId);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Service)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.ServiceId);

        modelBuilder.Entity<Category>()
         .HasData([
             new Category()
             {
                 Id = 1,
                 Name = "OkeConsultancy"
             },
             new Category()
             {
                 Id = 2,
                 Name = "EcoConsultancy"
             },
             new Category()
             {
                 Id = 3,
                 Name = "EcoSurvay"
             },
             new Category()
             {
                 Id = 4,
                 Name = "ForestSurvay"
             }
             ]);

        modelBuilder.Entity<Service>()
            .HasData(new[] {
                new Service()
                {
                    Id = 1,
                    Name = "Oke Consultancy Product 1",
                    Editing =true,
                    CategoryId = 1,
                    //ImageUrl = "",
                    Price = 999.9M,
                    Description = "Description for Oke Consultancy Product 1"
                },
                new Service()
                {
                    Id = 2,
                    Name = "Eco Consultancy Product 1",
                    Editing = true,
                    CategoryId = 2,
                    //ImageUrl = "",
                    Price = 10.0M,
                    Description = "Description for Eco Consultancy Product 1"
                },
                new Service()
                {
                    Id = 3,
                    Name = "Eco Survay Product 1",
                    Editing = true,
                    CategoryId = 3,
                    //ImageUrl = "",
                    Price = 999.9M,
                     Description = "Description for Eco Eco Survay Product 1"
                },
                new Service()
                {
                    Id = 4,
                    Name = "Forest Survay Product 1",
                    Editing = true,
                    CategoryId = 4,
                    //ImageUrl = "",
                    Price = 10.0M,
                    Description = "Description for Forest Survay Product 1"
                }
            });

        modelBuilder.Entity<CheckListItem>()
            .HasData(new CheckListItem[]
            {
                new CheckListItem()
                {
                    Id = 1,
                    Name = "Tree Age",
                    SurveyId = 1
                },
                new CheckListItem()
                {
                    Id = 2,
                    Name = "Stem diameter",
                    SurveyId = 1
                },
                new CheckListItem()
                {
                    Id = 3,
                    Name = "Tree height",
                    SurveyId = 1
                },
                new CheckListItem()
                {
                    Id = 4,
                    Name = "Tree Health status",
                    SurveyId = 1
                },
                new CheckListItem()
                {
                    Id = 5,
                    Name = "Tree location",
                    SurveyId = 1
                },
                new CheckListItem()
                {
                    Id = 6,
                    Name = "Tree Number",
                    SurveyId = 1
                },
                new CheckListItem()
                {
                    Id = 7,
                    Name = "Tree Type",
                    SurveyId = 1
                }
            });

        modelBuilder.Entity<Order>().HasData(new[]
{
    new Order
    {
        Id = 1,
        UserId = 1,
        ServiceId = 1,
        OrderDate = DateTime.Now,
        Status = "Pending"
    },
    new Order
    {
        Id = 2,
        UserId = 1,
        ServiceId = 2,
        OrderDate = DateTime.Now,
        Status = "Completed"
    }
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
