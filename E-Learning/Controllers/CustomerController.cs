using System.Security.Claims;
using E_Learning.Data;
using E_Learning.DTOs;
using E_Learning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Learning.Controllers;

[Authorize(Roles = "Customer")]
[Route("api/customer")]
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
                Course = course,
                Quantity = 1 
            });
        }
        else
        {
            cartCourse.Quantity++;
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
            cart = new Cart
            {
                CustomerId = customerId,
                CartCourses = new List<CartCourse>()
            };
            _dbContext.Carts.Add(cart);
        }

        var cartCoursesDto = cart.CartCourses.Select(cc => new CourseDTO
        {
            CourseId = cc.CourseId, 
            CourseTile = cc.Course.CourseTile,
            CourseDescription = cc.Course.CourseDescription,
            CoursePrice = cc.Course.CoursePrice,
            Category = cc.Course.Category,
            Quantity = cc.Quantity
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
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customerId == null)
        {
            return Unauthorized();
        }

        var cart = await _dbContext.Carts
            .Include(c => c.CartCourses)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart == null)
        {
            return NotFound();
        }

        var cartCourse = cart.CartCourses.FirstOrDefault(cc => cc.CourseId == courseId);
        if (cartCourse == null)
        {
            return NotFound();
        }

        if (cartCourse.Quantity > 1)
        {
            cartCourse.Quantity--;
        }
        else
        {
            _dbContext.CartCourses.Remove(cartCourse);
        }

        await _dbContext.SaveChangesAsync();

        return Ok("Course quantity has been updated in the cart successfully");
    }


    [HttpPut("updateCart/{courseId}/{quantity}")]
    public async Task<IActionResult> UpdateCart(string courseId, int quantity)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customerId == null)
        {
            return Unauthorized();
        }

        var cart = await _dbContext.Carts
            .Include(c => c.CartCourses)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart == null)
        {
            return NotFound("Cart not found");
        }

        var cartCourse = cart.CartCourses.FirstOrDefault(cc => cc.CourseId == courseId);
        if (cartCourse == null)
        {
            return NotFound("CartCourse not found");
        }

        if (quantity <= 0)
        {
            _dbContext.CartCourses.Remove(cartCourse);
        }
        else
        {
            cartCourse.Quantity = quantity;
        }

        await _dbContext.SaveChangesAsync();

        return Ok("Cart has been updated successfully");
    }
    
    [HttpPost("CheckoutCourses")]
    public async Task<IActionResult> CheckoutCourses()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customerId == null)
        {
            return Unauthorized();
        }

        var cart = await _dbContext.Carts
            .Include(c => c.CartCourses)
            .ThenInclude(cc => cc.Course)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart == null)
        {
            return NotFound("Cart not found");
        }

        foreach (var cartCourse in cart.CartCourses)
        {
            var existingUserCourse = await _dbContext.UserCourses
                .FirstOrDefaultAsync(uc => uc.CustomerId == customerId && uc.CourseId == cartCourse.CourseId);

            if (existingUserCourse != null)
            {
                existingUserCourse.Quantity += cartCourse.Quantity;
            }
            else
            {
                var userCourse = new UserCourse
                {
                    CustomerId = customerId,
                    CourseId = cartCourse.CourseId,
                    Course = cartCourse.Course,
                    Quantity = cartCourse.Quantity
                };

                _dbContext.UserCourses.Add(userCourse);
            }
        }

        cart.CartCourses.Clear();

        await _dbContext.SaveChangesAsync();

        return Ok("Courses have been added to your purchased courses, and the cart has been cleared.");
    }

    [HttpGet("userCourses")]
    public async Task<IActionResult> GetUserCourses()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customerId == null)
        {
            return Unauthorized();
        }

        var userCourses = await _dbContext.UserCourses
            .Where(uc => uc.CustomerId == customerId)
            .Include(uc => uc.Course)
            .ToListAsync();

        if (userCourses == null || userCourses.Count == 0)
        {
            return NotFound("No courses found for this user.");
        }

        // Select the data you want to return
        var courseData = userCourses.Select(uc => new CourseDTO
        {
            CourseId = uc.CourseId,
            CourseTile = uc.Course.CourseTile,
            CourseDescription = uc.Course.CourseDescription,
            CoursePrice = uc.Course.CoursePrice,
            Category = uc.Course.Category,
            Quantity = uc.Quantity
        });

        return Ok(courseData);
    }


}