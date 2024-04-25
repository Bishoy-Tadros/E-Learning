namespace E_Learning.Models;

public class Cart
{
    public string CartId { get; set; } = Guid.NewGuid().ToString();
    public string CustomerId { get; set; }
    public Customer Customer { get; set; }
    public List<CartCourse> CartCourses { get; set; }
}