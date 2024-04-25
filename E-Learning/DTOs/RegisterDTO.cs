using System.ComponentModel.DataAnnotations;

namespace E_Learning.DTOs;

public class RegisterDTO
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
    
    [Required]
    public string Address { get; set; }
    
    [Required]
    public string Area { get; set; }
    
    [Required]
    public string PhoneNumber { get; set; }

    
    
    
    
    
    
    
}