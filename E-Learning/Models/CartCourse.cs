namespace E_Learning.Models;

public class CartCourse
{
    public string CartId { get; set; }
    public Cart Cart { get; set; }
    public string CourseId { get; set; }
    public Course Course { get; set; }
}