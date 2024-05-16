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
    private readonly ITokenService _tokenService;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
         ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register/{userRole}")]
    public async Task<IActionResult> Register(RegisterDTO registerDto, string userRole)
    {
        var user = new User();
        
        if (userRole.ToLower() == "customer")
        {
            userRole = "Customer";
            user = new Customer
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                Address = registerDto.Address,
                Area = registerDto.Area,
                PhoneNumber = registerDto.PhoneNumber
            };
        }
        else if (userRole.ToLower() == "admin")
        {
            userRole = "Admin";
            user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                Address = registerDto.Address,
                Area = registerDto.Area,
                PhoneNumber = registerDto.PhoneNumber
            };
        }
        else
        {
            return BadRequest("Invalid user role");
        }


        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, userRole);

            if (roleResult.Succeeded)
            {
                await _signInManager.CheckPasswordSignInAsync(user, registerDto.Password, false);
                var roles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.CreateToken(user, roles);

                return Ok(new LoginResponseDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Token = token,
                        UserRole = userRole
                    }
                );
            }
        }

        return BadRequest(result.Errors);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
       
        if (user == null) 
            return Unauthorized("Invalid username!");
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        
        if (!result.Succeeded)
        {
            return Unauthorized("Username not found and/or password incorrect");
        }
        
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
                    UserRole = roles[0]
                });
        }

        return Unauthorized();
    }
    
}