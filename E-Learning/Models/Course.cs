namespace E_Learning.Models;

public class Course
{
    public string CourseId { get; set; } = Guid.NewGuid().ToString();
    public string CourseTile { get; set; }
    public string CourseDescription { get; set; }
    public double CoursePrice { get; set; }
    public string Category { get; set; }
    public List<UserCourse> UserCourses { get; set; }
    public List<CartCourse> CartCourses { get; set; }
}