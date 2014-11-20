using System;
using Demo2.Api.Resources;
using Demo2.Domain.Entities;

namespace Demo2.Api.Mappers
{
    public class User2UserResourceMapper : IMapper<User, UserResource>
    {
        public UserResource Map(User source)
        {
            var result = new UserResource
            {
                Name = source.FirstName + " " + source.LastName,
                AddressLine = source.Address.AddressLine,
                Email = source.Email,
                City = source.Address.City,
                Zip = source.Address.Zip,                
                Id = Convert.ToInt32(source.Id.Split('/')[1])
            };
            return result;
        }
    }
}