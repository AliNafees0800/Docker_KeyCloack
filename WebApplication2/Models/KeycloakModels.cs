using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class KeycloakCallbackData
    {
        [Required]
        public string SessionState { get; set; } = string.Empty;
        
        [Required]
        public string Iss { get; set; } = string.Empty;
        
        [Required]
        public string Code { get; set; } = string.Empty;
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? TokenData { get; set; }
    }

    public class TokenRequest
    {
        public string GrantType { get; set; } = "authorization_code";
        public string ClientId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
    }

    public class KeycloakUserInfo
    {
        public string Sub { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PreferredUsername { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();
    }

    public class KeycloakConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Realm { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
    }
}

