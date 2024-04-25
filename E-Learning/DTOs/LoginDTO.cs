using System.ComponentModel.DataAnnotations;

namespace E_Learning.DTOs;

public class LoginDTO
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

}