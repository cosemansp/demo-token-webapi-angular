using System;
using System.Reflection;
using Demo2.Api.Controllers;
using Demo2.Api.Resources;
using Demo2.Domain.Entities;

namespace Demo2
{
    public class Article2ArticleResourceMapper : IMapper<Article, ArticleResource>
    {
        public ArticleResource Map(Article source)
        {
            var result = new ArticleResource
            {
                Name = source.Name,
                Price = source.Price,
                Id =  Convert.ToInt32(source.Id.Split('/')[1])
            };
            return result;
        }
    }
}