using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Admin;

namespace Dashboard.Controllers.MVC
{
    [Authorize]
    public class AdminController : RazorController
    {
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult Index()
        {
            return View("~/Views/Admin.cshtml", new AdminViewModel());
        }
    }
}
