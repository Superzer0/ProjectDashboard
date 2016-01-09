using System.Web.Http;
using Dashboard.Infrastructure.ActionResults;
using Dashboard.Infrastructure.Razor;

namespace Dashboard.Infrastructure.Controllers
{
    public class RazorController : ApiController
    {
        public IExecuteRazorViews ExecuteRazorViews { get; set; }

        protected RazorResult View(string fullPath, object model)
        {
            return new RazorResult(model, fullPath, Request, ExecuteRazorViews);
        }
    }
}
