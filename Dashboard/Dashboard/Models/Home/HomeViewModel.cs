using System.Collections;
using System.Collections.Generic;
using Dashboard.Models.Account;
using Dashboard.UI.Objects.Auth;

namespace Dashboard.Models.Home
{
    public class HomeViewModel : BaseViewModel
    {
        public IEnumerable<PluginViewModel> Plugins { get; set; }
        public DashboardUser User { get; set; }
    }
}
