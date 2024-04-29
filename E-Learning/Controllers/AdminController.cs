using E_Learning.Data;
using E_Learning.DTOs;
using E_Learning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Learning.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _dbContext;

    public AdminController(UserManager<User> userManager, SignInManager<User> signInManager,
        ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = applicationDbContext;
    }

    [HttpPost("addCourse")]
    public async Task<IActionResult> AddCourse(AddCourseDTO addCourseDto)
    {
        var addedCourse = new Course
        {
            CourseTile = addCourseDto.CourseTile,
            CoursePrice = addCourseDto.CoursePrice,
            CourseDescription = addCourseDto.CourseDescription,
            Category = addCourseDto.Category
        };
        _dbContext.Courses.Add(addedCourse);
        await _dbContext.SaveChangesAsync();
        return Ok("Course has been added successfully");
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

    [HttpPut("editCourse/{courseId}")]
    public async Task<IActionResult> EditCourse(string courseId, CourseDTO courseDto)
    {
        var existingCourse = await _dbContext.Courses.FindAsync(courseId);
        if (existingCourse == null)
        {
            return NotFound();
        }

        existingCourse.CourseTile = courseDto.CourseTile;
        existingCourse.CourseDescription = courseDto.CourseDescription;
        existingCourse.CoursePrice = courseDto.CoursePrice;
        existingCourse.Category = courseDto.Category;

        _dbContext.Courses.Update(existingCourse);
        await _dbContext.SaveChangesAsync();

        return Ok("Course has been edited successfully");
    }

    [HttpDelete("deleteCourse/{courseId}")]
    public async Task<IActionResult> DeleteCourse(string courseId)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course == null)
        {
            return NotFound();
        }

        _dbContext.Courses.Remove(course);
        await _dbContext.SaveChangesAsync();

        return Ok("Course has been deleted successfully");
    }
}