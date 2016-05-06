using System;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.Owin;

namespace Dashboard.Infrastructure.Middleware
{
    internal class GlobalExceptionLoggerMiddleware : OwinMiddleware
    {
        private readonly ILog _log = LogManager.GetLogger<GlobalExceptionLoggerMiddleware>();

        public GlobalExceptionLoggerMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception e)
            {
                _log.Error(m => m("Unhandled exception: "), e);
                throw;
            }
        }
    }
}
