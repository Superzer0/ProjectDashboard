using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Account;
using Dashboard.Models.ApplicationAccess;
using Dashboard.UI.Objects.Auth;
using Dashboard.UI.Objects.Services;

namespace Dashboard.Controllers.API
{
    [RoutePrefix("api/manageApps")]
    [Authorize(Roles = DashboardRoles.Admin)]
    public class ApplicationAccessController : BaseController
    {
        private readonly IAuthRepository _authRepository;

        public ApplicationAccessController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpGet]
        [Route("refreshTokens")]
        public IHttpActionResult GetRefreshTokens()
        {
            var authRefreshTokens = _authRepository.GetAllRefreshTokens().Where(r => r.ExpiresUtc > DateTime.UtcNow); ;
            return Ok(authRefreshTokens);
        }

        [HttpDelete]
        [Route("refreshTokens/{tokenId}")]
        public async Task<IHttpActionResult> Delete(string tokenId)
        {
            var result = await _authRepository.RemoveRefreshToken(tokenId);
            if (result)
            {
                return Ok();
            }

            return BadRequest("Token Id does not exist");
        }

        [HttpGet]
        [Route("apps")]
        public IHttpActionResult GetApps()
        {
            var authClients = _authRepository.GetAllClients();
            var authRefreshTokens = _authRepository.GetAllRefreshTokens().Where(r => r.ExpiresUtc > DateTime.UtcNow);

            return Ok(authClients.Select(p =>
                    new RegisteredClientApplications
                    {
                        Client = p,
                        ActiveUsers = authRefreshTokens.Count(r => r.ClientId == p.Id)
                    }
                ));
        }

        [HttpPost]
        [Route("app/create")]
        public IHttpActionResult CreateApp(CreateAppViewModel appViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var newApp = _authRepository.CreateClient(appViewModel.Name, appViewModel.ApplicationType,
                appViewModel.AllowedOrigin);
                return Ok(newApp);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete]
        [Route("apps/{appId:guid}")]
        public async Task<IHttpActionResult> DeleteApp(string appId)
        {
            var actionResult = await _authRepository.DeleteClient(appId);
            if (actionResult)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("apps/status/{appId:guid}/{status}")]
        public async Task<IHttpActionResult> DeactivateApp(string appId, bool status)
        {
            var actionResult = await _authRepository.ToggleAppState(appId, status);
            if (actionResult)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("apps/secret/regenerate/{appId:guid}")]
        public async Task<IHttpActionResult> RegenerateAppSecret(string appId)
        {
            try
            {
                return Ok(await _authRepository.ReGenerateAppSecret(appId));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}

