using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo2.Domain.Services;
using Microsoft.Owin;
using Owin;

namespace Demo2.Web
{
    public class JwtOwinAuthOptions
    {
        public string Secret { get; set; }
        public bool Disable { get; set; }
    }

    public static class AppBuilderExtensions
    {
        public static void UseJwtOwinAuth(this IAppBuilder app, JwtOwinAuthOptions options)
        {
            app.Use<JwtAuthMiddleware>(options);
        }
    }

    public class JwtAuthMiddleware : OwinMiddleware
    {
        private readonly JwtOwinAuthOptions _options;

        public JwtAuthMiddleware(OwinMiddleware next, JwtOwinAuthOptions options)
            : base(next)
        {
            _options = options;
        }

        public override Task Invoke(IOwinContext context)
        {
            var headers = context.Environment["owin.RequestHeaders"] as IDictionary<string, string[]>;
            if (headers == null)
                return Next.Invoke(context);

            if (headers.ContainsKey("Authorization"))
            {
                string token = GetTokenFromAuthorizationHeader(headers["Authorization"]);
                try
                {
                    var jwtClaims = JsonWebToken.DecodeToObject<JsonWebTokenClaims>(token, _options.Secret);
                    if (jwtClaims.IsExpired)
                    {
                        return UnauthorizedResponse(context);
                    }

                    // valid token 
                    var principal = new ClaimsPrincipal(jwtClaims.ToClaimsIdentity());
                    context.Request.User = principal;
                }
                catch (SignatureVerificationException)
                {
                    // invalid token
                    return UnauthorizedResponse(context);
                }
            }
            else if (_options.Disable)
            {
                var identity = new ClaimsIdentity("oauth2", "name", "role");
                identity.AddClaim(new Claim("name", "System Administrator"));
                identity.AddClaim(new Claim("role", "SysAdmin"));
                var principal = new ClaimsPrincipal(identity);
                context.Request.User = principal;
            }

            return Next.Invoke(context);
        }

        public string GetTokenFromAuthorizationHeader(string[] authorizationHeader)
        {
            // look for this header and return token
            // Authorization: Bearer 0b79bab50daca910b000d4f1a2b675d604257e42
            if (authorizationHeader.Length == 0)
            {
                throw new ApplicationException("Invalid authorization header. It must have at least one element");
            }
            string token = authorizationHeader[0].Split(' ')[1];
            return token;
        }

        public Task UnauthorizedResponse(IOwinContext context)
        {
            context.Environment["owin.ResponseStatusCode"] = 401;
            return Next.Invoke(context);
        }
    }
}