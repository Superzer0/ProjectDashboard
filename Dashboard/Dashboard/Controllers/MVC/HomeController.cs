using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Razor;
using Dashboard.Models.Home;

namespace Dashboard.Controllers.MVC
{
    public class HomeController : RazorController
    {
        private readonly ILog _log = LogManager.GetLogger<HomeController>();

        public HomeController(IExecuteRazorViews razorEngineService)
        {
            
        }

        // GET: api/Home
        public IHttpActionResult Get()
        {
            return View("../Views/Home.cshtml", new HomeViewModel { Version = "0.1v" });
        }

        // GET: api/Home/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Home
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Home/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Home/5
        public void Delete(int id)
        {
        }
    }
}
