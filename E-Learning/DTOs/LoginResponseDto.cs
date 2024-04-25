using System.ComponentModel.DataAnnotations;

namespace E_Learning.DTOs;

    public class LoginResponseDto
    {

        public string Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

        public string UserRole { get; set; }
    }

