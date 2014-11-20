using System.ComponentModel.DataAnnotations;

namespace Demo2.Api.Resources
{
    public class RefreshTokenResource
    {
        [Required]
        public string Token { get; set; }
    }
}