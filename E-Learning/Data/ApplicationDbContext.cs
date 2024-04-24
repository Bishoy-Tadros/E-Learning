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

    private DbSet<User> Users { get; set;}
    private DbSet<Course> Courses { get; set;}
    private DbSet<UserCourse> UserCourses { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserCourse>()
            .HasKey(uc => new { uc.UserId, uc.CourseId });

        modelBuilder.Entity<UserCourse>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UserCourses)
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserCourse>()
            .HasOne(uc => uc.Course)
            .WithMany(c => c.UserCourses)
            .HasForeignKey(uc => uc.CourseId);
        
        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"


            },

            new IdentityRole
            {
                Name = "User",
                NormalizedName = "USER"


            },
            
        };


        modelBuilder.Entity<IdentityRole>().HasData(roles);

    }

}