using System.ComponentModel.DataAnnotations;

namespace Demo2.Api.Resources
{
    public class AuthorizeResource
    {
        [Required]
        public string ApiKey { get; set; }
    }
}