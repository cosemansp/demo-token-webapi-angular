using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Raven.Client;

namespace Demo2
{
    public interface IHaveADocumentSession
    {
        IDocumentSession Session { get; set; }
    }

    public class RavenDbActionFilter : ActionFilterAttribute
    {
        private readonly IDocumentStore _docStore;

        public RavenDbActionFilter(IDocumentStore docStore)
        {
            _docStore = docStore;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var controller = actionContext
                                .ControllerContext
                                .Controller as IHaveADocumentSession;
            if (controller == null)
                return;

            controller.Session = _docStore.OpenSession();

            //Console.WriteLine("pre test filter");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var controller = actionExecutedContext
                                .ActionContext
                                .ControllerContext
                                .Controller as IHaveADocumentSession;

            if (controller == null)
                return;

            controller.Session.SaveChanges();
            controller.Session.Dispose();

            //Console.WriteLine("post test filter");
        }
    }
}