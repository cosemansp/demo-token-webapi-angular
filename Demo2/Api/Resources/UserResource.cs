using System.ComponentModel.DataAnnotations;

namespace Demo2.Api.Resources
{
    public class UserResource
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string AddressLine { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }

    }
}