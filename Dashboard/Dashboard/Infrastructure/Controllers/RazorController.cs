using System.Dynamic;
using Dashboard.Infrastructure.ActionResults;
using Dashboard.Infrastructure.Razor;
using Dashboard.Models;

namespace Dashboard.Infrastructure.Controllers
{
    public class RazorController : BaseController
    {
        protected readonly dynamic ViewBag = new ExpandoObject();

        public IExecuteRazorViews ExecuteRazorViews { get; set; }

        protected RazorResult View(string fullPath, object model)
        {
            SetCommonViewBagProperties();
            var viewModel = model as BaseViewModel;
            if (viewModel != null)
            {
                viewModel.Url = Url;
            }

            var mappedPath = Environment.MapPath(fullPath);
            return new RazorResult(model, mappedPath, Request, ExecuteRazorViews, ViewBag);
        }

        private void SetCommonViewBagProperties()
        {
            ViewBag.AppVersion = Environment.AppVersion;
            ViewBag.BaseAdress = Environment.BaseAddress;
        }
    }
}
