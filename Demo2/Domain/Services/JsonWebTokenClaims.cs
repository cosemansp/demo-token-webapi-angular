using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Demo2.Domain.Entities;

namespace Demo2.Domain.Services
{
    public class JsonWebTokenClaims : Dictionary<string, object>
    {
        public string Roles
        {
            get
            {
                if (ContainsKey("roles"))
                    return this["roles"].ToString();
                return string.Empty;
            }
        }

        public string Name
        {
            get
            {
                if (ContainsKey("name"))
                    return this["name"].ToString();
                return string.Empty;
            }
        }

        public DateTime ExpireTime
        {
            get
            {
                if (ContainsKey("exp"))
                {
                    var expireTimeInSeconds = (int) this["exp"];
                    var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return utc0.AddSeconds(expireTimeInSeconds);
                }
                return DateTime.MaxValue;
            }
        }

        public bool IsExpired
        {
            get { return (ExpireTime < DateTime.Now); }
        }

        public ClaimsIdentity ToClaimsIdentity()
        {
            var claims = new List<System.Security.Claims.Claim>();
            foreach (var key in Keys)
            {
                // exclude following keys: just for internal JWT handling
                if (key == "iss" || key == "iat" || key == "sub" || key == "exp")
                    continue;

                if (this[key] == null)
                    continue;  // safeguard

                if (key == "roles")
                {
                    // special case for roles, we have to convert the csv to multiple claims
                    var value = this[key].ToString();
                    var roles = value.Split(',');
                    claims.AddRange(roles.Select(role => new Claim("role", role)));
                }
                else
                {
                    // default: claim from token = claim to identity
                    var value = this[key].ToString();
                    claims.Add(new Claim(key, value));
                }
            }
            return new ClaimsIdentity(claims, "oauth", "name", "role");
        }

        public JsonWebTokenClaims()
        {
        }

        public JsonWebTokenClaims(string userId)
        {
            var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var issueTime = DateTime.Now;
            var iat = (int) issueTime.Subtract(utc0).TotalSeconds;
            Add("iss", "euricom"); // JTW - the issuer of the claim
            Add("iat", iat); // JTW - Issued-at time
            Add("sub", "demo"); // JTW - The subject of this token
            Add("userId", userId);
        }

        public JsonWebTokenClaims(User user, int expireMinutes)
        {
            var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var issueTime = DateTime.Now;
            var iat = (int) issueTime.Subtract(utc0).TotalSeconds;
            var exp = (int) issueTime.AddMinutes(expireMinutes).Subtract(utc0).TotalSeconds;
            Add("iss", "euricom"); // JTW - the issuer of the claim
            Add("iat", iat); // JTW - Issued-at time
            Add("sub", "demo"); // JTW - The subject of this token
            Add("exp", exp); // JTW - Expiration time;
            Add("roles", user.Roles);
            Add("name", user.FullName);
            Add("userId", user.Id);

            //foreach (var claim in user.Claims)
            //{
            //    Add(claim.Type, claim.Value);
            //}
        }
    }
}