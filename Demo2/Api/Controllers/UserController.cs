using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Demo2.Api.Mappers;
using Demo2.Api.Resources;
using Demo2.Domain.Entities;
using Demo2.Domain.Services;
using Raven.Client;
using Raven.Client.Linq;

namespace Demo2.Api.Controllers
{
    [RoutePrefix("api/users")]
    public class UserController : ApiController, IHaveADocumentSession
    {
        private readonly IMapper<User, UserResource> _userMapper;
        private readonly IMapper<User.ApiKey, ApiKeyResource> _apiKeyMapper;

        public IDocumentSession Session { get; set; }

        public UserController()
        {
            _userMapper = new User2UserResourceMapper();
            _apiKeyMapper = new ApiKey2ApiKeyResourceMapper();
        }

        // Poor man dependency injection
        public UserController(IMapper<User, UserResource> userMapper,
                              IMapper<User.ApiKey, ApiKeyResource> apiKeyMapper)
        {
            _userMapper = userMapper;
            _apiKeyMapper = apiKeyMapper;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            Console.WriteLine("GET");

            // get from db
            var users = Session.Query<User>().ToList();

            // map to resources
            var resources = users.Select(x => _userMapper.Map(x))
                                    .ToList();
            // return 200
            return Ok(resources);
        }

        // GET /api/users/1234
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            Console.WriteLine("GET: {0}", id);

            // get from db
            var user = Session.Load<User>(id);
            if (user == null)
                return NotFound();

            // map
            var resources = _userMapper.Map(user);

            // return 200
            return Ok(resources);
        }

        [Route("{userId}/apikeys")]
        public IHttpActionResult GetApiKeys(int userId)
        {
            // get from db
            var user = Session.Load<User>(userId);
            if (user == null)
                return NotFound();

            // map
            var resources = user.ApiKeys.Select(x => _apiKeyMapper.Map(x))
                                        .ToList();
            // return 200
            return Ok(resources);
        }

        [Route("{userId}/apikeys")]
        public IHttpActionResult PostApiKey(int userId, ApiKeyCreateResource resource)
        {
            // validate
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // get from db
            var user = Session.Load<User>(userId);
            if (user == null)
                return NotFound();

            // create new apiKey
            var apiKeyExpireDate = DateTime.Now + TimeSpan.FromDays(90);
            var apiKey = ApiKeyFactory.GenerateRandom(apiKeyExpireDate);
            user.ApiKeys.Add(new User.ApiKey
            {
                Name = resource.Name,
                ExpireDate = apiKeyExpireDate,
                EncryptedKey = ApiKeyFactory.Encrypt(apiKey)
            });

            // store to db
            Session.Store(user);

            // map
            var result = new
            {
                ApiKey = apiKey,
                Name = resource.Name,
                Expire = apiKeyExpireDate
            };

            // return 200
            return Ok(result);
        }

        [Route("{id}/apikeys/{keyName}")]
        public HttpResponseMessage DeleteApiKeys(int id, string keyName)
        {
            // look for user
            var user = Session.Load<User>(id);
            if (user == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // look for key on user
            var apiKey = user.ApiKeys.FirstOrDefault(x => x.Name == keyName);
            if (apiKey == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);

            // remove apikey from user
            user.ApiKeys.Remove(apiKey);

            // store to db
            Session.Store(user);

            // map
            var resource = _apiKeyMapper.Map(apiKey);

            // return 200
            return Request.CreateResponse(HttpStatusCode.OK, resource);
        }

    }
}