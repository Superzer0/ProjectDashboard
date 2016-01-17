using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Common.Logging;

namespace Dashboard.Infrastructure.Filters
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute, IAutofacExceptionFilter
    {
        private readonly ILog _logger = LogManager.GetLogger<ErrorHandlingFilter>();
        public new void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            _logger.Error(actionExecutedContext.Exception);

            if (actionExecutedContext.Exception is NotImplementedException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
