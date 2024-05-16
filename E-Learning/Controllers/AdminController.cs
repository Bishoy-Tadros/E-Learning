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

    private readonly ApplicationDbContext _dbContext;

    public AdminController(ApplicationDbContext applicationDbContext)
    {
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

    

    [HttpPut("editCourse/{courseId}")]
    public async Task<IActionResult> EditCourse(string courseId, EditCourseDTO editCourseDto)
    {
        var existingCourse = await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (existingCourse == null)
        {
            return NotFound();
        }

        existingCourse.CourseTile = editCourseDto.CourseTile;
        existingCourse.CourseDescription = editCourseDto.CourseDescription;
        existingCourse.CoursePrice = editCourseDto.CoursePrice;
        existingCourse.Category = editCourseDto.Category;

        _dbContext.Courses.Update(existingCourse);
        await _dbContext.SaveChangesAsync();

        return Ok("Course has been edited successfully");
    }

    [HttpDelete("deleteCourse/{courseId}")]
    public async Task<IActionResult> DeleteCourse(string courseId)
    {
        var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course == null)
        {
            return NotFound();
        }

        _dbContext.Courses.Remove(course);
        await _dbContext.SaveChangesAsync();

        return Ok("Course has been deleted successfully");
    }
}