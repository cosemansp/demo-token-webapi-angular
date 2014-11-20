using System;
using System.Collections.Generic;
using System.Linq;
using Demo2.Domain.Entities;
using Raven.Client;

namespace Demo2.Domain.Services
{
    public interface IAuthenticateService
    {
        string AuthorizeByApiKey(string apiKey, int tokenExpireTime);
        string AuthorizeByUserPassword(string email, string passw, int tokenExpireTime);
        string AuthorizeByToken(string accessToken, int tokenExpireTime);
        string CreateRefreshToken(string token);
    }

    public class AuthenticateService : IAuthenticateService
    {
        private readonly IDocumentSession _dbSession;

        public AuthenticateService(IDocumentSession dbSession)
        {
            _dbSession = dbSession;
        }

        public string AuthorizeByUserPassword(string email, string passw, int expireMinutes)
        {
            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var user = _dbSession.Query<User>()
                                 .Where(x => x.Email == email)  // for simplicity we only query on the email
                                 .FirstOrDefault();

            if (user == null)
                return null;

            var newPayload = new JsonWebTokenClaims(user, expireMinutes);
            return JsonWebToken.Encode(newPayload, Config.SecretkeyJwt, JwtHashAlgorithm.HS256);
        }


        public string AuthorizeByApiKey(string apiKey, int expireMinutes)
        {
            var encryptedKey = ApiKeyFactory.Encrypt(apiKey);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var user = _dbSession.Query<User>()
                                 .Where(x => x.ApiKeys.Any(y => y.EncryptedKey == encryptedKey))
                                 .FirstOrDefault();

            if (user == null)
                return null;

            var newPayload = new JsonWebTokenClaims(user, expireMinutes);
            return JsonWebToken.Encode(newPayload, Config.SecretkeyJwt, JwtHashAlgorithm.HS256);
        }

        public string CreateRefreshToken(string token)
        {
            Console.WriteLine("CreateRefreshToken - token: {0}", token);
            try
            {
                // decode and verify 
                var payload = JsonWebToken.DecodeToObject<Dictionary<string, object>>(token, Config.SecretkeyJwt, true);
                var userId = payload["userId"].ToString();
                Console.WriteLine("   UserId: {0}", userId);

                var newPayload = new JsonWebTokenClaims(userId);
                return JsonWebToken.Encode(newPayload, Config.SecretkeyJwt, JwtHashAlgorithm.HS256);
            }
            catch (SignatureVerificationException ex)
            {
                // invalid token
                Console.WriteLine("   Token is invalid: {0}", ex.Message);
                return null;
            }
        }

        public string AuthorizeByToken(string refreshToken, int expireMinutes)
        {
            Console.WriteLine("AuthorizeByToken - token: {0}", refreshToken);

            try
            {
                // decode and verify 
                var payload = JsonWebToken.DecodeToObject<Dictionary<string, object>>(refreshToken, Config.SecretkeyJwt, true);

                // get user by id
                var userId = payload["userId"].ToString();
                var user = _dbSession.Load<User>(userId);
                if (user == null)
                {
                    Console.WriteLine("   User doesn't exist anymore: {0}", userId);
                    return null;
                }

                var newPayload = new JsonWebTokenClaims(user, expireMinutes);
                return JsonWebToken.Encode(newPayload, Config.SecretkeyJwt, JwtHashAlgorithm.HS256);
            }
            catch (SignatureVerificationException ex)
            {
                // invalid token
                Console.WriteLine("   Token is invalid: {0}", ex.Message);
                return null;
            }
        }
    }
}