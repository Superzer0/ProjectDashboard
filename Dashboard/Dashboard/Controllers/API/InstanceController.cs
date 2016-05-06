using System;
using System.Web.Http;
using Common.Logging;
using Dashboard.Infrastructure.Controllers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.BrokerIntegration;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/instance")]
    [Authorize(Roles = DashboardRoles.Admin)]
    public class InstanceController : BaseController
    {
        private readonly ILog _logger = LogManager.GetLogger<InstanceController>();
        private readonly IManageBrokerFacade _manageBrokerFacade;

        public InstanceController(IManageBrokerFacade manageBrokerFacade)
        {
            _manageBrokerFacade = manageBrokerFacade;
        }

        [Route("broker-status")]
        public IHttpActionResult GetBrokerStatus()
        {
            try
            {
                var brokerInformation = _manageBrokerFacade.GetBrokerInformation();
                return Ok(brokerInformation);
            }
            catch (Exception e)
            {
                _logger.Error("[Broker not responding]", e);
                return InternalServerError();
            }
        }

    }
}
