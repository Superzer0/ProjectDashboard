using System;
using System.Linq;
using System.Web.Http;
using Common.Logging;
using Dashboard.DataAccess;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Razor;
using Dashboard.Models.Home;
using Dashboard.UI.Objects.DataObjects;

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
            using (var context = new PluginsContext())
            {
                var plugin = new Plugin { Id = Guid.NewGuid(), AddedBy = "Kuba", Added = DateTime.Now, Xml = "hehe" };
                context.Plugins.Add(plugin);
                context.SaveChanges();
                var elements = context.Plugins.Count();
                return View("../Views/Home.cshtml", new HomeViewModel { Version = elements.ToString() });
            }
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
