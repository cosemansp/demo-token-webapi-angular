using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Demo2.Api.Resources;
using Demo2.Domain.Entities;
using Raven.Client;

namespace Demo2.Api.Controllers
{
    [RoutePrefix("api/articles")]
    public class ArticleController : ApiController, IHaveADocumentSession
    {
        private readonly IMapper<Article, ArticleResource> _articleMapper;

        // Poor man dependency injection
        public ArticleController(IMapper<Article, ArticleResource> mapper = null)
        {
            _articleMapper = mapper ?? new Article2ArticleResourceMapper();
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            Console.WriteLine("GET");

            // get from db
            var articles = Session.Query<Article>().ToList();

            // map to resources
            var resources = articles.Select(x => _articleMapper.Map(x))
                                    .ToList();
            // return 200
            return Ok(resources);
        }

        // GET /api/articles/1234
        [Route("{id}", Name = "getById")]
        public IHttpActionResult Get(int id)
        {
            Console.WriteLine("GET: {0}", id);

            // get from db
            var article = Session.Load<Article>(id);
            if (article == null)
                return NotFound();

            // map
            var resource = _articleMapper.Map(article);

            // return 200
            return Ok(resource);
        }

        [Route("")]
        //[Authorize(Roles = "admin")]
        public IHttpActionResult Post(ArticleResource resource)
        {
            // validate
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // create new article
            var article = new Article()
            {
                Name = resource.Name,
                Price = resource.Price
            };

            // store
            Session.Store(article);

            // map article to resource
            var updatedResource = _articleMapper.Map(article);

            // return 201
            return Created(Url.Link("getById", new {id = updatedResource.Id}), 
                           updatedResource);
        }

        // PUT /api/articles/1234
        [Route("{id}")]
        //[Authorize(Roles = "admin")]
        public IHttpActionResult Put(int id, ArticleResource resource)
        {
            // validate
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // get from db
            var article = Session.Load<Article>(id);
            if (article == null)
                return NotFound();

            // update article
            article.Name = resource.Name;
            article.Price = resource.Price;

            // strore to db
            Session.Store(article);

            // map
            var result = _articleMapper.Map(article);

            return Ok(result);
        }

        // DELETE /api/articles/1234
        [Route("{id}")]
        //[Authorize(Roles = "admin")]
        //[ClaimsAuthorizeFilter("articles", "d")]
        public HttpResponseMessage Delete(int id)
        {
            // find article
            var article = Session.Load<Article>(id);
            if (article == null)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            
            // delete from db
            Session.Delete(article);

            // map to resource
            var resource = _articleMapper.Map(article);

            // return 200
            return Request.CreateResponse(HttpStatusCode.OK, resource);
        }

        public IDocumentSession Session { get; set; }
    }
}