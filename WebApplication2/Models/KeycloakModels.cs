using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication2.Models
{
    public class KeycloakCallbackData
    {
        [Required]
        [JsonPropertyName("session_state")]
        public string SessionState { get; set; } = string.Empty;
        
        [Required]
        [JsonPropertyName("iss")]
        public string Iss { get; set; } = string.Empty;
        
        [Required]
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
    }

    public class ValidationResult
    {
        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        
        [JsonPropertyName("token_data")]
        public object? TokenData { get; set; }
    }

    public class TokenRequest
    {
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "authorization_code";
        
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;
        
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
        
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; } = string.Empty;
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
        
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;
        
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = string.Empty;
        
        [JsonPropertyName("session_state")]
        public string SessionState { get; set; } = string.Empty;
        
        [JsonPropertyName("not-before-policy")]
        public int NotBeforePolicy { get; set; }
    }

    public class KeycloakUserInfo
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = string.Empty;
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("preferred_username")]
        public string PreferredUsername { get; set; } = string.Empty;
        
        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }
        
        [JsonPropertyName("roles")]
        public string[] Roles { get; set; } = Array.Empty<string>();
    }

    public class KeycloakConfig
    {
        [JsonPropertyName("base_url")]
        public string BaseUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("realm")]
        public string Realm { get; set; } = string.Empty;
        
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;
        
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;
        
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; } = string.Empty;
    }
}

