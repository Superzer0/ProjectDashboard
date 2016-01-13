using System.Web.Http;
using Dashboard.Infrastructure.Controllers;

namespace Dashboard.Controllers.MVC
{
    [RoutePrefix("admin")]
    public class AdminController : RazorController
    {
        [Route("")]
        [HttpGet]
        public IHttpActionResult Index()
        {
            return View("../Views/Admin.cshtml", new object());
        }
    }
}
