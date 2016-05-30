using System;
using System.Web.Http;
using Dashboard.Infrastructure.Controllers;
using Dashboard.Models.Admin;
using Dashboard.UI.Objects.Services;

namespace Dashboard.Controllers.MVC
{
    [Authorize]
    public class AdminController : RazorController
    {
        private readonly IAuthRepository _authRepository;

        public AdminController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult Index()
        {
            return View("~/Views/Admin.cshtml", new AdminViewModel());
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("authConstants")]
        public IHttpActionResult AuthConstants()
        {
            var officialAppClient = _authRepository.GenerateOfficialClientId();
            if (string.IsNullOrWhiteSpace(officialAppClient?.Id))
            {
                throw new NullReferenceException("officialAppClient id corrupted. Application cannot work properly");
            }

            return View("~/Views/AuthConstants.cshtml", officialAppClient.Id);
        }
    }
}
