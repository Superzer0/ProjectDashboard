using System.Linq;
using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Infrastructure.Identity;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/instance")]
    [Authorize(Roles = DashboardRoles.Admin)]
    public class InstanceController : BaseController
    {
       

    }
}
