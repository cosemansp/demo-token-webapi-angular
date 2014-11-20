using System.ComponentModel.DataAnnotations;

namespace Demo2.Api.Resources
{
    public class ApiKeyCreateResource
    {
        [Required]
        public string Name { get; set; }
    }
}