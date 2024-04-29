using E_Learning.Data;
using E_Learning.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Learning.Controllers;

[Route("api/course")]
[Authorize]
[ApiController]
public class CourseController: ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public CourseController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet("getAllCourses")]
    public async Task<IActionResult> GetAllCourses()
    {
        var courses = await _dbContext.Courses.Select(c => new ViewCourseDTO
        {
            CourseTile = c.CourseTile,
            CourseDescription = c.CourseDescription,
            CoursePrice = c.CoursePrice,
            Category = c.Category,
            CourseId = c.CourseId
        }).ToListAsync();

        return Ok(courses);
    }
    
    
    [HttpGet("viewCourse/{courseId}")]
    public async Task<IActionResult> ViewCourse(string courseId)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);

        var courseDetails = new CourseDTO
        {
            CourseId = course.CourseId,
            CourseTile = course.CourseTile,
            CourseDescription = course.CourseDescription,
            Category = course.Category,
            CoursePrice = course.CoursePrice,
        };
        return Ok(courseDetails);
    }

}