namespace E_Learning.Models;

public class UserCourse
{
    public string CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    public string CourseId { get; set; }
    public Course Course { get; set; }
}