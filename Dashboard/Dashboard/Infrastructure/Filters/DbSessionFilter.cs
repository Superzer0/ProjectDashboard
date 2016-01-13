using System.Data;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Dashboard.DataAccess;

namespace Dashboard.Infrastructure.Filters
{
    public class DbSessionFilter : IAutofacActionFilter
    {
        private readonly PluginsContext _pluginsContext;

        public DbSessionFilter(PluginsContext pluginsContext)
        {
            _pluginsContext = pluginsContext;
        }

        public void OnActionExecuting(HttpActionContext actionContext)
        {
            _pluginsContext.Database.BeginTransaction(IsolationLevel.Serializable);
        }

        public void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            using (var transaction = _pluginsContext.Database.CurrentTransaction)
            {
                if (transaction == null) return;

                if (actionExecutedContext.Exception != null)
                {
                    transaction.Rollback();
                }
                else
                {
                    transaction.Commit();
                }
            }
        }
    }
}
