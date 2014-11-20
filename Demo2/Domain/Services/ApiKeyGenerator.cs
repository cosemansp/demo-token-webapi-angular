using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Demo2.Domain.Services
{
    public static class ApiKeyFactory
    {
        private const string EncryptKey = "z935dbvzrdX0Z9625240H28R74mA1N4E2yZE4UY7Jp0P8D61lqdfdsfkQ30il635Xc41VueFU58zI0BAQ2Nu3N945S6402w6eM40Us";

        public static string Encrypt(string apiKey)
        {
            var hashObject = new HMACSHA256(Encoding.UTF8.GetBytes(EncryptKey));
            var apiKeyEncrypted = hashObject.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
            return Convert.ToBase64String(apiKeyEncrypted);
        }

        public static string GenerateRandom(DateTime expireDate)
        {
            var segments = new List<string>();

            var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var exp = Convert.ToString((long)expireDate.Subtract(utc0).TotalSeconds);

            const string apiIdentifier = "API-";

            // GenerateRandom a new globally unique identifier for the salt
            var salt = Guid.NewGuid().ToString();


            byte[] saltRandomBytes = Encoding.UTF8.GetBytes(salt);
            segments.Add(Convert.ToBase64String(saltRandomBytes));
            segments.Add(exp);

            var hashObject = new HMACSHA256();

            var stringToSign = string.Join(".", segments.ToArray());

            // Computes the signature by hashing the salt with the secret key as the key
            var signature = hashObject.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));

            // Base 64 GenerateRandom
            var encodedSignature = Convert.ToBase64String(signature);

            // API-base64(apiKeyInfo).encodedSignature
            var apiKeyResult = apiIdentifier + exp + "." + encodedSignature;

            return apiKeyResult;
        }
    }
}