namespace E_Learning.Models;

public class Customer : User
{
    public List<UserCourse> UserCourses { get; set; }
    public Cart Cart { get; set; }
}