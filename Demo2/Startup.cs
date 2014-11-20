using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Demo2.Data.Indexes;
using Demo2.Domain.Entities;
using Demo2.Web;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json.Serialization;
using Owin;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Demo2
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // init ravendb
            var docStore = new DocumentStore
            {
                ConnectionStringName = "db.connection.raven"
            };
            docStore.Initialize();
            IndexCreation.CreateIndexes(typeof(UserIndex).Assembly, docStore);

            var user1 = new User()
            {
                FirstName = "Bart",
                LastName = "Jansens",
                Email = "bart.jansens@euri.com",
                Address = new Address()
                {
                    AddressLine = "Polderlaan 8",
                    City = "Zoeterkompen",
                    Zip = " 4785"
                },
                Roles = "admin, user",
                Claims = new List<User.Claim>()
                {
                    new User.Claim()
                    {
                        Type = "article",
                        Value = "rwd"
                    },
                    new User.Claim()
                    {
                        Type = "user",
                        Value = "rwd"
                    }
                }
            };

            var user2 = new User()
            {
                FirstName = "John",
                LastName = "Doo",
                Email = "john.doo@euri.com",
                Address = new Address()
                {
                    AddressLine = "Ikkemersstraat 2345",
                    City = "Zierieken",
                    Zip = " B1234"
                },
                Roles = "user",
                Claims = new List<User.Claim>()
                {
                    new User.Claim()
                    {
                        Type = "article",
                        Value = "r"
                    }
                }
            };

            using (var session = docStore.OpenSession())
            {
                if (session.Query<User>().Count() == 0)
                {
                    session.Store(user1);
                    session.Store(user2);
                    session.SaveChanges();
                }
            }


            // config http
            var config = new HttpConfiguration();

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = 
                new CamelCasePropertyNamesContractResolver();

            // Enable attributes routing
            config.MapHttpAttributeRoutes();

            // Setup default routes
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            // handle local file system (static files)
            app.UseFileServer(new FileServerOptions()
            {
                FileSystem = new PhysicalFileSystem("../../client"),
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = true
            });

            // handle JWT authentication
            app.UseJwtOwinAuth(new JwtOwinAuthOptions
            {
                Secret = Config.SecretkeyJwt,
                Disable = false
            });

            // enable CORS
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // add global filters
            config.Filters.Add(new RavenDbActionFilter(docStore));

            // use WebAPI
            app.UseWebApi(config);
        }
    }
}