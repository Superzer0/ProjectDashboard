using System;
using System.Linq;
using System.Web.Http;
using Common.Logging;
using Dashboard.DataAccess;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Home;
using Dashboard.UI.Objects.DataObjects;
using Microsoft.AspNet.Identity;

namespace Dashboard.Controllers.MVC
{
    public class HomeController : RazorController
    {
        private readonly PluginsContext _pluginsContext;
        private readonly ILog _log = LogManager.GetLogger<HomeController>();

        public HomeController(PluginsContext pluginsContext)
        {
            _pluginsContext = pluginsContext;
        }

        // GET: api/Home
        public IHttpActionResult Index()
        {
            var plugin = new Plugin { Id = Guid.NewGuid(), AddedBy = "Kuba", Added = DateTime.Now, Xml = "hehe" };
            _pluginsContext.Plugins.Add(plugin);
            _pluginsContext.SaveChanges();
            var elements = _pluginsContext.Plugins.Count();

            _log.Info(p => p((_pluginsContext.Database.CurrentTransaction != null).ToString()));
            var userId = User.Identity.GetUserId();


            return View("../Views/Home.cshtml", new HomeViewModel { Version = userId });
        }

        // GET: api/Home/5
        public string Index(int id)
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
