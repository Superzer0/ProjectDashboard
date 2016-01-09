using System;
using System.Collections.Generic;
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
                DisableTempFileLocking = true,
                EncodedStringFactory = new HtmlEncodedStringFactory(),
                Debug = false,
                TemplateManager = new ResolvePathTemplateManager(_viewsLocations),
            };

            _razorEngineService = RazorEngineService.Create(config);
            _logger.Info(m => m("Razor Engine created"));
        }

        public string Execute(string viewPath, object model, string layoutPath)
        {
            throw new NotImplementedException();
        }

        public string ExecutePartial(string viewPath, object model)
        {
            return _razorEngineService.RunCompile(new FullPathTemplateKey(viewPath, viewPath, ResolveType.Global, null),
                 model.GetType(), model);
        }
    }
}
