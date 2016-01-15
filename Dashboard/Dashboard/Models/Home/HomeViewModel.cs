using System.Collections;
using System.Collections.Generic;
using Dashboard.Models.Account;

namespace Dashboard.Models.Home
{
    public class HomeViewModel : BaseViewModel
    {
        public IEnumerable<PluginViewModel> Plugins { get; set; }
        public DashboardUser User { get; set; }
    }
}
