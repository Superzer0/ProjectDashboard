namespace Dashboard.UI.Objects.Services
{
    public interface IConfigureDashboard
    {
        bool GetAdminPartyState();
        void SetAdminPartyState(bool changeStatus);
    }
}
