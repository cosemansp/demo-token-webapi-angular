using System.Web.Http;
using Demo2.Api.Resources;
using Demo2.Domain.Services;
using Raven.Client;

namespace Demo2.Api.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController, IHaveADocumentSession
    {
        public const int TokenExpireTime = 60; // minutes

        public IDocumentSession Session { get; set; }

        [Route("login")]
        public IHttpActionResult PostLogin(LoginResource resource)
        {
            // validate
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // get access token, based on valid apiKey
            var authenticateService = new AuthenticateService(Session);
            var accessToken = authenticateService.AuthorizeByUserPassword(resource.Email, resource.Password, TokenExpireTime);
            if (accessToken == null)
                return Unauthorized();

            // based on the accessToken, create a valid refresh token
            var refreshtoken = authenticateService.CreateRefreshToken(accessToken);

            // return access & refresh token
            var result = new
            {
                AccessToken = accessToken,
                RefreshToken = refreshtoken,
                Expire = TokenExpireTime * 60, /* expire in seconds */
                Type = "bearer"
            };

            return Ok(result);
        }


        [Route("authorize")]
        public IHttpActionResult PostAuthorize(AuthorizeResource resource)
        {
            // validate
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // get access token, based on valid apiKey
            var authenticateService = new AuthenticateService(Session);
            var accessToken = authenticateService.AuthorizeByApiKey(resource.ApiKey, TokenExpireTime);
            if (accessToken == null)
                return Unauthorized();

            // based on the accessToken, create a valid refresh token
            var refreshtoken = authenticateService.CreateRefreshToken(accessToken);

            // return access & refresh token
            var result = new
            {
                AccessToken = accessToken,
                RefreshToken = refreshtoken,
                Expire = TokenExpireTime * 60, /* expire in seconds */
                Type = "bearer"
            };

            return Ok(result);
        }

        [Route("refreshToken")]
        public IHttpActionResult PostAuthorize(RefreshTokenResource resource)
        {
            // validate
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // based on a valid refresh token,
            // create a new access token
            var authenticateService = new AuthenticateService(Session);
            var accessToken = authenticateService.AuthorizeByToken(resource.Token, TokenExpireTime);
            if (accessToken == null)
                return Unauthorized();

            // return new access token
            var result = new
            {
                AccessToken = accessToken,
                Expire = TokenExpireTime * 60, /* expire in seconds */
                Type = "bearer"
            };

            return Ok(result);
        }
    }
}