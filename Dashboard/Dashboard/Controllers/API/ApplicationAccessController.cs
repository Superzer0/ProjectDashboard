using System.Threading.Tasks;
using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Account;
using Dashboard.UI.Objects.Services;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/refreshtokens")]
//    [Authorize(Users = DashboardRoles.Admin)]
    public class ApplicationAccessController : BaseController
    {
        private readonly IAuthRepository _authRepository;

        public ApplicationAccessController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_authRepository.GetAllRefreshTokens());
        }
        
        [Route("")]
        public async Task<IHttpActionResult> Delete(string tokenId)
        {
            var result = await _authRepository.RemoveRefreshToken(tokenId);
            if (result)
            {
                return Ok();
            }

            return BadRequest("Token Id does not exist");
        }

        [HttpPost]
        [Route("createapp")]
        public IHttpActionResult CreateApp(CreateAppViewModel appViewModel)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var newApp = _authRepository.CreateClient(appViewModel.Name, appViewModel.ApplicationType, appViewModel.AllowedOrigin);
            return Ok(newApp);
        }

        [HttpGet]
        [Route("apps")]
        public IHttpActionResult GetApps()
        {
            return Ok(_authRepository.GetAllClients());
        }
    }
}
