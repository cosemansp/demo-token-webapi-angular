using Demo2.Api.Resources;
using Demo2.Domain.Entities;

namespace Demo2.Api.Mappers
{
    public class ApiKey2ApiKeyResourceMapper : IMapper<User.ApiKey, ApiKeyResource>
    {
        public ApiKeyResource Map(User.ApiKey source)
        {
            var result = new ApiKeyResource
            {
                Name = source.Name,
                ExpireDate = source.ExpireDate
            };
            return result;
        }
    }
}