namespace E_Learning.Models;

public class Cart
{
    public string CartId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public User User { get; set; }
    public List<CartCourse> CartCourses { get; set; }
}