using System.Collections.Generic;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.DataObjects.Display;

namespace Dashboard.Models.Home
{
    public class HomeViewModel : BaseViewModel
    {
        public ProcessedPluginHtml[] Plugins { get; set; }
        public DashboardUser User { get; set; }
    }
}
