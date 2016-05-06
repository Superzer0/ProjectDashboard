using System;
using System.Threading.Tasks;
using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.DataObjects.Execution;
using Dashboard.UI.Objects.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dashboard.Controllers.API
{
    [Authorize(Roles = DashboardRoles.User)]
    [RoutePrefix("api/dispatch")]
    public class PluginCallDispatchController : BaseController
    {
        private readonly ICallRemoteMethods _remoteMethods;

        public PluginCallDispatchController(ICallRemoteMethods remoteMethods)
        {
            _remoteMethods = remoteMethods;
        }

        [HttpPost]
        [Route("{pluginId:guid}/{version}/{method}")]
        public async Task<IHttpActionResult> RemoteCall(string pluginId, string version, string method, [FromBody] JToken parameters)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(version) || string.IsNullOrWhiteSpace(method))
                return BadRequest($"{nameof(version)} or {nameof(method)} params cannot be empty");

            var currentUser = await GetCurrentUser();

            if (currentUser == null) return BadRequest("user not found");

            var brokerCall = new BrokerExecutionInfo
            {
                Version = version,
                MethodName = method,
                Parameters = parameters?.ToString() ?? string.Empty,
                PluginId = pluginId
            };

            try
            {
                var response = await _remoteMethods.CallRemoteMethod(brokerCall, currentUser.Id);
                var deserialized = JsonConvert.DeserializeObject(response);
                return Ok(deserialized);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
