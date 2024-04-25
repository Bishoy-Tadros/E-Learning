using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace E_Learning.Models;

public class User : IdentityUser
{
    public string Address { get; set; }
    
    public string Area { get; set; }
    
    public List<UserCourse> UserCourses { get; set; }
    public Cart Cart { get; set; }
}