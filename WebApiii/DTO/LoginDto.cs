using System.ComponentModel.DataAnnotations;


namespace WebApiii.DTO
    {
        public class LoginDTO
        {
        [Required]
            public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
        }
   }



