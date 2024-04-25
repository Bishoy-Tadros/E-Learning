﻿using E_Learning.Data;
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
        var courses = await _dbContext.Courses.Select(c => new CourseDTO
        {
            CourseTile = c.CourseTile,
            CourseDescription = c.CourseDescription,
            CoursePrice = c.CoursePrice,
            Category = c.Category
        }).ToListAsync();

        return Ok(courses);
    }

}