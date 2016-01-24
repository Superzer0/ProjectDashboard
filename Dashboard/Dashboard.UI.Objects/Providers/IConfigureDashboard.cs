namespace Dashboard.UI.Objects.Providers
{
    public interface IConfigureDashboard
    {
        bool GetAdminPartyState();
        void SetAdminPartyState(bool changeStatus);
    }
}
