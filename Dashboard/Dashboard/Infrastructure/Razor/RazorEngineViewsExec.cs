using System.Collections.Generic;
using System.Dynamic;
using Common.Logging;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Dashboard.Infrastructure.Razor
{
    internal class RazorEngineViewsExec : IExecuteRazorViews
    {
        private readonly ILog _logger = LogManager.GetLogger<RazorEngineViewsExec>();
        private readonly IRazorEngineService _razorEngineService;
        private readonly IEnumerable<string> _viewsLocations = new[] { @"../Views", @"../Views/Shared" };

        public RazorEngineViewsExec()
        {
            var config = new TemplateServiceConfiguration
            {
                Language = Language.CSharp,
                DisableTempFileLocking = false,
                EncodedStringFactory = new HtmlEncodedStringFactory(),
                Debug = false,
                TemplateManager = new ResolvePathTemplateManager(_viewsLocations),
            };

            _razorEngineService = RazorEngineService.Create(config);
            _logger.Info(m => m("Razor Engine created"));
        }

        public string Execute(string viewPath, object model, dynamic viewBag, string layoutPath)
        {
            return _razorEngineService.RunCompile(new FullPathTemplateKey(viewPath, viewPath, ResolveType.Global, null),
                        model.GetType(), model, new DynamicViewBag((ExpandoObject)viewBag));
        }

        public string ExecutePartial(string viewPath, object model, dynamic viewBag)
        {
            return _razorEngineService.RunCompile(new FullPathTemplateKey(viewPath, viewPath, ResolveType.Global, null),
                model.GetType(), model, new DynamicViewBag((ExpandoObject)viewBag));
        }
    }
}
