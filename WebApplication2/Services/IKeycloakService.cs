using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IKeycloakService
    {
        Task<ValidationResult> ValidateCallbackAsync(KeycloakCallbackData callbackData);
        Task<TokenResponse?> ExchangeCodeForTokenAsync(KeycloakCallbackData callbackData);
        Task<KeycloakUserInfo?> GetUserInfoAsync(string accessToken);
        Task<bool> ValidateTokenAsync(string accessToken);
        Task<ValidationResult> ValidateSessionStateAsync(string sessionState, string code);
    }
}

