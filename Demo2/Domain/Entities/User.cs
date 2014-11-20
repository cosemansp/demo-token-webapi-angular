using System;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json;

namespace Demo2.Domain.Entities
{
    public class Address
    {
        public string AddressLine { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
    }

    public class User
    {
        public class ApiKey
        {
            public string Name { get; set; }
            public string EncryptedKey { get; set; }
            public DateTime ExpireDate { get; set; }
        }

        public class Claim
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public List<ApiKey> ApiKeys { get; set; }
        public List<Claim> Claims { get; set; }
        public string Roles { get; set; }

        public User()
        {
            Address = new Address();
            ApiKeys = new List<ApiKey>();
        }

        [JsonIgnore]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}