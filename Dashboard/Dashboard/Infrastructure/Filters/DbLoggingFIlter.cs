using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Common.Logging;
using Dashboard.DataAccess;

namespace Dashboard.Infrastructure.Filters
{
    public class DbLoggingFilter : IAutofacActionFilter
    {
        private readonly PluginsContext _pluginsContext;
        private readonly ILog _logger = LogManager.GetLogger<DbLoggingFilter>();

        public DbLoggingFilter(PluginsContext pluginsContext)
        {
            _pluginsContext = pluginsContext;
        }

        public void OnActionExecuting(HttpActionContext actionContext)
        {
            _pluginsContext.Database.Log = message => _logger.Debug(m => m(message));
        }

        public void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

        }
    }
}
