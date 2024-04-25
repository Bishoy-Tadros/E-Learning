using E_Learning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Learning.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<User> Users { get; set;}
    public DbSet<Customer> Customers { get; set;}
    public DbSet<Course> Courses { get; set;}
    public DbSet<UserCourse> UserCourses { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartCourse> CartCourses { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserCourse>()
            .HasKey(uc => new { uc.CustomerId, uc.CourseId });

        modelBuilder.Entity<UserCourse>()
            .HasOne(uc => uc.Customer)
            .WithMany(u => u.UserCourses)
            .HasForeignKey(uc => uc.CustomerId);

        modelBuilder.Entity<UserCourse>()
            .HasOne(uc => uc.Course)
            .WithMany(c => c.UserCourses)
            .HasForeignKey(uc => uc.CourseId);
        
        
        modelBuilder.Entity<CartCourse>()
            .HasKey(cc => new { cc.CartId, cc.CourseId });

        modelBuilder.Entity<CartCourse>()
            .HasOne(cc => cc.Cart)
            .WithMany(c => c.CartCourses)
            .HasForeignKey(cc => cc.CartId);

        modelBuilder.Entity<CartCourse>()
            .HasOne(cc => cc.Course)
            .WithMany(c => c.CartCourses)
            .HasForeignKey(cc => cc.CourseId);

        modelBuilder.Entity<Customer>()
            .HasOne(u => u.Cart)
            .WithOne(c => c.Customer)
            .HasForeignKey<Cart>(c => c.CustomerId);
        
        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"


            },

            new IdentityRole
            {
                Name = "Customer",
                NormalizedName = "CUSTOMER"


            },
            
        };


        modelBuilder.Entity<IdentityRole>().HasData(roles);

    }

}