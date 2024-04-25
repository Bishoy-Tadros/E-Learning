using E_Learning.DTOs;
using E_Learning.Interfaces;
using E_Learning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Learning.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
        IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
    }

    [HttpPost("register/{userRole}")]
    public async Task<IActionResult> Register(RegisterDTO registerDto, string userRole)
    {
        if (userRole.ToLower() == "user")
        {
            userRole = "User";
        }
        else if (userRole.ToLower() == "admin")
        {
            userRole = "Admin";
        }
        else
        {
            return BadRequest("Invalid user role");
        }

        var user = new User
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            Address = registerDto.Address,
            Area = registerDto.Area,
            PhoneNumber = registerDto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, userRole);

            if (roleResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                var roles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.CreateToken(user, roles);

                return Ok(new LoginResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = token,
                }
                );
            }
        }

        return BadRequest(result.Errors);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());
        var result =
            await _signInManager.PasswordSignInAsync(loginDto.Username, loginDto.Password, isPersistent: true, lockoutOnFailure: false);
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.CreateToken(user, roles);

        if (result.Succeeded)
        {
            return Ok(
                new LoginResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = token,
                });
        }

        return Unauthorized();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _httpContextAccessor.HttpContext.Session.Clear();
        return Ok();
    }
}