using System.Security.Claims;
using E_Learning.Data;
using E_Learning.DTOs;
using E_Learning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Learning.Controllers;

[Authorize(Roles = "User")]
[Route("api/user")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _dbContext;

    public CustomerController(UserManager<User> userManager, SignInManager<User> signInManager,
        ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = applicationDbContext;
    }

    [HttpGet("viewCourse/{courseId}")]
    public async Task<IActionResult> ViewCourse(string courseId)
    {
        var course = await _dbContext.Courses.FindAsync(courseId);

        var courseDetails = new CourseDTO
        {
            CourseTile = course.CourseTile,
            CourseDescription = course.CourseDescription,
            Category = course.Category,
            CoursePrice = course.CoursePrice,
        };
        return Ok(courseDetails);
    }

    [HttpPost("addToCart/{courseId}")]
    public async Task<IActionResult> AddToCart(string courseId)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customerId == null)
        {
            return Unauthorized();
        }

        var course = await _dbContext.Courses.FindAsync(courseId);
        if (course == null)
        {
            return NotFound();
        }

        var cart = await _dbContext.Carts
            .Include(c => c.CartCourses)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart == null)
        {
            cart = new Cart
            {
                CustomerId = customerId,
                CartCourses = new List<CartCourse>()
            };
            _dbContext.Carts.Add(cart);
        }

        var cartCourse = cart.CartCourses.FirstOrDefault(cc => cc.CourseId == courseId);
        if (cartCourse == null)
        {
            cart.CartCourses.Add(new CartCourse
            {
                CartId = cart.CartId,
                CourseId = courseId,
                Course = course
            });
        }

        await _dbContext.SaveChangesAsync();

        return Ok("Course has been added to the cart successfully");
    }




    [HttpGet("viewCart")]
    public IActionResult ViewCart()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customerId == null)
        {
            return Unauthorized();
        }

        var cart = _dbContext.Carts
            .Include(c => c.CartCourses)
            .ThenInclude(cc => cc.Course)
            .FirstOrDefault(c => c.CustomerId == customerId);

        if (cart == null)
        {
            return NotFound("Cart not found.");
        }

        var cartCoursesDto = cart.CartCourses.Select(cc => new CourseDTO
        {
            CourseTile = cc.Course.CourseTile,
            CourseDescription = cc.Course.CourseDescription,
            CoursePrice = cc.Course.CoursePrice,
            Category = cc.Course.Category 
        }).ToList();

        var cartDto = new
        {
            CartId = cart.CartId,
            CartCourses = cartCoursesDto
        };

        return Ok(cartDto);
    }


    [HttpDelete("deleteFromCart/{courseId}")]
    public async Task<IActionResult> DeleteFromCart(string courseId)
    {
        var customerId = _userManager.GetUserId(User);
        var cart = _dbContext.Carts.Include(c => c.CartCourses).FirstOrDefault(c => c.CustomerId == customerId);
        var cartCourse = cart.CartCourses.FirstOrDefault(cc => cc.CourseId == courseId);
        if (cartCourse == null)
        {
            return NotFound();
        }

        _dbContext.CartCourses.Remove(cartCourse);
        await _dbContext.SaveChangesAsync();

        return Ok("Course has been deleted from the cart successfully");
    }

    [HttpPut("updateCart/{courseId}/{quantity}")]
    public async Task<IActionResult> UpdateCart(string courseId, int quantity)
    {
        var customerId = _userManager.GetUserId(User);
        var cart = _dbContext.Carts.Include(c => c.CartCourses).FirstOrDefault(c => c.CustomerId == customerId);
        var cartCourse = cart.CartCourses.FirstOrDefault(cc => cc.CourseId == courseId);
        if (cartCourse == null)
        {
            return NotFound();
        }

        _dbContext.CartCourses.Update(cartCourse);
        await _dbContext.SaveChangesAsync();

        return Ok("Cart has been updated successfully");
    }
}