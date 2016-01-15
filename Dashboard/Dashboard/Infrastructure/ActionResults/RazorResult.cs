using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Dashboard.Infrastructure.Razor;

namespace Dashboard.Infrastructure.ActionResults
{
    public class RazorResult : IHttpActionResult
    {
        private readonly object _model;
        private readonly dynamic _viewBag;
        private readonly string _viewPath;
        private readonly IExecuteRazorViews _razorEngine;
        private readonly HttpRequestMessage _requestMessage;

        public RazorResult(object model, string viewPath, HttpRequestMessage requestMessage,
            IExecuteRazorViews razorEngine, dynamic viewBag)
        {
            _model = model;
            _requestMessage = requestMessage;
            _viewPath = viewPath;
            _razorEngine = razorEngine;
            _viewBag = viewBag;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var htmlContent = _razorEngine.ExecutePartial(_viewPath, _model, _viewBag);

            var response = new HttpResponseMessage
            {
                Content = new StringContent(htmlContent, Encoding.UTF8, "text/html"),
                StatusCode = HttpStatusCode.OK,
                RequestMessage = _requestMessage
            };

            return Task.FromResult(response);
        }
    }
}
