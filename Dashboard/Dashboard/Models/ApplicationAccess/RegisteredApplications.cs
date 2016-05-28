using Dashboard.UI.Objects.Auth;

namespace Dashboard.Models.ApplicationAccess
{
    public class RegisteredClientApplications
    {
        public AuthClient Client { get; set; }
        public int ActiveUsers { get; set; }
    }
}
