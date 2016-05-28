using System.Collections.Generic;

namespace Dashboard.Infrastructure.Identity.Server
{
    internal static class AuthError
    {
        public static AuthErrorDescription AgentErrorClientIdMissing => new AuthErrorDescription
        {
            ErrorKey = "invalid_clientId",
            ErrorDescription = "ClientId should be sent."
        };

        public static AuthErrorDescription AgentErrorClientIdNotFound => new AuthErrorDescription
        {
            ErrorKey = "invalid_clientId",
            ErrorDescription = "ClientId is not registered in the system."
        };

        public static AuthErrorDescription AgentErrorSecretMissing => new AuthErrorDescription
        {
            ErrorKey = "invalid_clientId",
            ErrorDescription = "Client secret should be sent."
        };

        public static AuthErrorDescription AgentErrorSecretInvalid => new AuthErrorDescription
        {
            ErrorKey = "invalid_clientId",
            ErrorDescription = "Client secret is invalid."
        };

        public static AuthErrorDescription AgentErrorAppInactive => new AuthErrorDescription
        {
            ErrorKey = "invalid_clientId",
            ErrorDescription = "Client is inactive."
        };

        public static AuthErrorDescription AgentErrorRefreshTokenMismatch => new AuthErrorDescription
        {
            ErrorKey = "invalid_clientId",
            ErrorDescription = "Refresh token is issued to a different clientId."
        };

        public static AuthErrorDescription PasswordGrantCredentialsInvalid => new AuthErrorDescription
        {
            ErrorKey = "invalid_grant",
            ErrorDescription = "The user name or password is incorrect"
        };

        public static AuthErrorDescription UserNotFoundForRefreshToken => new AuthErrorDescription
        {
            ErrorKey = "invalid_grant",
            ErrorDescription = "User for this refresh token no longer exist."
        };
    }

    internal struct AuthErrorDescription
    {
        public string ErrorKey { get; set; }
        public string ErrorDescription { get; set; }
    }
}
