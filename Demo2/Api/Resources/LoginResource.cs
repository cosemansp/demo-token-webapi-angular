using System.ComponentModel.DataAnnotations;

namespace Demo2.Api.Resources
{
    public class LoginResource
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}