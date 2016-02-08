using System.Threading.Tasks;

namespace Dashboard.UI.Objects.Providers
{
    public interface IManagePlugins
    {
        Task SwitchPluginInstanceState(string appId, string version, bool state);
        Task SwitchPluginUserState(string appId, string version, string user, bool state);
        Task ChangeUserPluginConfiguration(string appId, string version, string user, string configuration);
    }
}
