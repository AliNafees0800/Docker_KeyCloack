using System.Text;
using System.Text.Json;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class KeycloakService : IKeycloakService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeycloakService> _logger;
        private readonly KeycloakConfig _keycloakConfig;

        public KeycloakService(HttpClient httpClient, IConfiguration configuration, ILogger<KeycloakService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _keycloakConfig = _configuration.GetSection("Keycloak").Get<KeycloakConfig>() ?? new KeycloakConfig();
        }

        public async Task<ValidationResult> ValidateCallbackAsync(KeycloakCallbackData callbackData)
        {
            try
            {
                _logger.LogInformation("Validating Keycloak callback for session state: {SessionState}", callbackData.SessionState);

                // Basic validation
                if (string.IsNullOrEmpty(callbackData.SessionState) || 
                    string.IsNullOrEmpty(callbackData.Code) || 
                    string.IsNullOrEmpty(callbackData.Iss))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "Missing required parameters: session_state, code, or iss"
                    };
                }

                // Validate session state format
                var sessionValidation = await ValidateSessionStateAsync(callbackData.SessionState, callbackData.Code);
                if (!sessionValidation.IsValid)
                {
                    return sessionValidation;
                }

                // Validate issuer URL
                if (!IsValidIssuerUrl(callbackData.Iss))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "Invalid issuer URL format"
                    };
                }

                // Basic validation passed - don't exchange token here to avoid using code twice
                return new ValidationResult
                {
                    IsValid = true,
                    Message = "Callback validation successful - ready for token exchange"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Keycloak callback");
                return new ValidationResult
                {
                    IsValid = false,
                    Message = $"Validation error: {ex.Message}"
                };
            }
        }

        public async Task<TokenResponse?> ExchangeCodeForTokenAsync(KeycloakCallbackData callbackData)
        {
            try
            {
                var tokenUrl = $"{_keycloakConfig.BaseUrl}/realms/{_keycloakConfig.Realm}/protocol/openid-connect/token";
                
                var tokenRequest = new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "authorization_code"),
                    new("client_id", _keycloakConfig.ClientId),
                    new("client_secret", _keycloakConfig.ClientSecret),
                    new("code", callbackData.Code),
                    new("redirect_uri", _keycloakConfig.RedirectUri)
                };

                var formContent = new FormUrlEncodedContent(tokenRequest);
                
                // Log the request for debugging
                _logger.LogInformation("Exchanging code for token - URL: {TokenUrl}, ClientId: {ClientId}, RedirectUri: {RedirectUri}", 
                    tokenUrl, _keycloakConfig.ClientId, _keycloakConfig.RedirectUri);
                
                var response = await _httpClient.PostAsync(tokenUrl, formContent);

                // Log ALL response details for debugging
                _logger.LogInformation("=== KEYCLOAK RESPONSE DEBUG ===");
                _logger.LogInformation("Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Reason Phrase: {ReasonPhrase}", response.ReasonPhrase);
                _logger.LogInformation("Is Success: {IsSuccess}", response.IsSuccessStatusCode);
                _logger.LogInformation("Version: {Version}", response.Version);
                
                // Log all response headers
                _logger.LogInformation("Response Headers:");
                foreach (var header in response.Headers)
                {
                    _logger.LogInformation("  {HeaderName}: {HeaderValue}", header.Key, string.Join(", ", header.Value));
                }
                
                // Log content headers
                _logger.LogInformation("Content Headers:");
                foreach (var header in response.Content.Headers)
                {
                    _logger.LogInformation("  {HeaderName}: {HeaderValue}", header.Key, string.Join(", ", header.Value));
                }

                // Always read and log the response body first
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("=== RESPONSE BODY ===");
                _logger.LogInformation("Response Body: {ResponseBody}", responseBody);
                _logger.LogInformation("=== END RESPONSE BODY ===");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Success Response Body: {ResponseBody}", responseBody);
                    
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.LogInformation("Successfully exchanged code for token");
                    return tokenResponse;
                }
                else
                {
                    _logger.LogError("=== KEYCLOAK ERROR RESPONSE ===");
                    _logger.LogError("Status Code: {StatusCode}", response.StatusCode);
                    _logger.LogError("Reason Phrase: {ReasonPhrase}", response.ReasonPhrase);
                    _logger.LogError("Error Response Body: {ErrorBody}", responseBody);
                    
                    // Log the request details for debugging
                    _logger.LogError("=== REQUEST DETAILS ===");
                    _logger.LogError("Token URL: {TokenUrl}", tokenUrl);
                    _logger.LogError("Client ID: {ClientId}", _keycloakConfig.ClientId);
                    _logger.LogError("Redirect URI: {RedirectUri}", _keycloakConfig.RedirectUri);
                    _logger.LogError("Authorization Code: {Code}", callbackData.Code);
                    
                    // Log the exact form data being sent
                    var formData = string.Join("&", tokenRequest.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                    _logger.LogError("Form Data Sent: {FormData}", formData);
                    
                    // Log each individual parameter
                    _logger.LogError("Individual Parameters:");
                    foreach (var param in tokenRequest)
                    {
                        _logger.LogError("  {Key} = {Value}", param.Key, param.Value);
                    }
                    
                    _logger.LogError("=== END DEBUG INFO ===");
                    
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exchanging code for token");
                return null;
            }
        }

        public async Task<KeycloakUserInfo?> GetUserInfoAsync(string accessToken)
        {
            try
            {
                var userInfoUrl = $"{_keycloakConfig.BaseUrl}/realms/{_keycloakConfig.Realm}/protocol/openid-connect/userinfo";
                
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.GetAsync(userInfoUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonSerializer.Deserialize<KeycloakUserInfo>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.LogInformation("Successfully retrieved user info for subject: {Subject}", userInfo?.Sub);
                    return userInfo;
                }
                else
                {
                    _logger.LogError("Failed to get user info. Status: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user info");
                return null;
            }
        }

        public async Task<bool> ValidateTokenAsync(string accessToken)
        {
            try
            {
                var userInfo = await GetUserInfoAsync(accessToken);
                return userInfo != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return false;
            }
        }

        public async Task<ValidationResult> ValidateSessionStateAsync(string sessionState, string code)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrEmpty(sessionState) || string.IsNullOrEmpty(code))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "Session state and code are required"
                    };
                }

                // Validate format (basic checks)
                if (sessionState.Length < 10 || code.Length < 10)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "Invalid session state or code format"
                    };
                }

                // Additional validation could be added here, such as:
                // - Checking session state against stored sessions
                // - Validating code format against expected patterns
                // - Checking expiration times

                return new ValidationResult
                {
                    IsValid = true,
                    Message = "Session state validation successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating session state");
                return new ValidationResult
                {
                    IsValid = false,
                    Message = $"Session state validation error: {ex.Message}"
                };
            }
        }

        private bool IsValidIssuerUrl(string iss)
        {
            try
            {
                var decodedIss = Uri.UnescapeDataString(iss);
                var uri = new Uri(decodedIss);
                
                // Check if it's a valid Keycloak issuer URL format
                return uri.Scheme == "http" || uri.Scheme == "https" &&
                       uri.AbsolutePath.Contains("/realms/");
            }
            catch
            {
                return false;
            }
        }
    }
}

