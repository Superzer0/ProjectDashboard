using System.Web.Http;
using Dashboard.Infrastructure.Identity;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/user")]
    [Authorize(Roles = DashboardRoles.User)]
    public class UserController
    {
    }
}
