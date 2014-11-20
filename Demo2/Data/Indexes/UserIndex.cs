using System.Linq;
using Demo2.Domain.Entities;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Demo2.Data.Indexes
{
    public class UserIndex : AbstractIndexCreationTask<User>
    {
        public UserIndex()
        {
            Map = users => from user in users
                           select new
                           {
                               user.Id,
                               Name = user.FirstName + " " + user.LastName,
                               user.Email,
                               AddressLine = user.Address.AddressLine,
                               Zip = user.Address.Zip,
                               City = user.Address.City,
                               Terms = new object[]
                               {
                                   user.Id,
                                   user.FirstName,
                                   user.LastName,
                                   user.Email,
                                   user.Address.AddressLine,
                                   user.Address.City,
                                   user.Address.Zip
                               }
                           };

            Index("Terms", FieldIndexing.Analyzed);
        }
    }
}