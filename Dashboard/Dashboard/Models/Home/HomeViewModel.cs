using System;
using System.Collections.Generic;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.DataObjects.Display;

namespace Dashboard.Models.Home
{
    public class HomeViewModel : BaseViewModel
    {
        public IEnumerable<IEnumerable<Tuple<ProcessedPluginHtml, ProcessedPluginConfiguration>>> PackedPlugisGrid { get; set; }
        public DashboardUser User { get; set; }
        public string ConigurationJson { get; set; }
    }
}
