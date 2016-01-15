using System.Web.Http.Routing;

namespace Dashboard.Models
{
    public abstract class BaseViewModel
    {
        public UrlHelper Url { get; set; }
    }
}
