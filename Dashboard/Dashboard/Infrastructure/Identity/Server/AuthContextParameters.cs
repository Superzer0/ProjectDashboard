namespace Dashboard.Infrastructure.Identity.Server
{
    internal static class AuthContextParameters
    {
        public const string ClientId = "as:client_id";
        public const string AllowedOrigin = "as:clientAllowedOrigin";
        public const string AllowedRefreshTokenLifeTime = "as:clientRefreshTokenLifeTime";
        public const string UserNameKey = "userName";
    }
}
