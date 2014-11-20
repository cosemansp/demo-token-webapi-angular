using System.ComponentModel.DataAnnotations;

namespace Demo2.Api.Resources
{
    public class ArticleResource
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, 999)]
        public double Price { get; set; }
    }
}