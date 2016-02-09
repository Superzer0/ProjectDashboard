using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;
using Dashboard.Broker.Services;

namespace WindowsBrokerService
{
    partial class Dashboard : ServiceBase
    {
        private static ServiceHost _installationService;
        private static ServiceHost _managementService;
        private static ServiceHost _pluginsExecutionService;

        public Dashboard()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif
            StartInstallationService();
            StartManagementService();
            StartExecutorService();
        }

        protected override void OnStop()
        {
            StopInstallationService();
            StopManagementService();
            StopExecutorService();
        }

        private void StartInstallationService()
        {
            _installationService?.Close();
            _installationService = new ServiceHost(typeof(InstallationService));
            _installationService.Open();
        }

        private void StopInstallationService()
        {
            if (_installationService == null) return;

            _installationService.Close();
            _installationService = null;
        }


        private void StartManagementService()
        {
            _managementService?.Close();
            _managementService = new ServiceHost(typeof(BrokerManagementService));
            _managementService.Open();
        }

        private void StopManagementService()
        {
            if (_managementService == null) return;

            _managementService.Close();
            _managementService = null;
        }


        private void StartExecutorService()
        {
            _pluginsExecutionService?.Close();
            _pluginsExecutionService = new ServiceHost(typeof(PluginExecutor));
            _pluginsExecutionService.Open();
        }

        private void StopExecutorService()
        {
            if (_pluginsExecutionService == null) return;

            _pluginsExecutionService.Close();
            _pluginsExecutionService = null;
        }
    }
}
