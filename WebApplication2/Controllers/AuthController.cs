using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IKeycloakService _keycloakService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IKeycloakService keycloakService, ILogger<AuthController> logger)
        {
            _keycloakService = keycloakService;
            _logger = logger;
        }

        [HttpPost("validate-keycloak-callback")]
        public async Task<IActionResult> ValidateKeycloakCallback([FromBody] KeycloakCallbackData callbackData)
        {
            try
            {
                _logger.LogInformation("Received Keycloak callback validation request");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ValidationResult
                    {
                        IsValid = false,
                        Message = "Invalid request data",
                        TokenData = ModelState
                    });
                }

                var result = await _keycloakService.ValidateCallbackAsync(callbackData);
                
                if (result.IsValid)
                {
                    _logger.LogInformation("Keycloak callback validation successful");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Keycloak callback validation failed: {Message}", result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Keycloak callback validation");
                return StatusCode(500, new ValidationResult
                {
                    IsValid = false,
                    Message = "Internal server error during validation"
                });
            }
        }

        [HttpPost("exchange-code-for-token")]
        public async Task<IActionResult> ExchangeCodeForToken([FromBody] KeycloakCallbackData callbackData)
        {
            try
            {
                _logger.LogInformation("Received token exchange request");

                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid request data");
                }

                var tokenResponse = await _keycloakService.ExchangeCodeForTokenAsync(callbackData);
                
                if (tokenResponse != null)
                {
                    _logger.LogInformation("Token exchange successful");
                    return Ok(tokenResponse);
                }
                else
                {
                    _logger.LogWarning("Token exchange failed");
                    return BadRequest(new { message = "Failed to exchange code for token" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exchanging code for token");
                return StatusCode(500, new { message = "Internal server error during token exchange" });
            }
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] TokenValidationRequest request)
        {
            try
            {
                _logger.LogInformation("Received token validation request");

                if (string.IsNullOrEmpty(request.AccessToken))
                {
                    return BadRequest(new { message = "Access token is required" });
                }

                var isValid = await _keycloakService.ValidateTokenAsync(request.AccessToken);
                
                return Ok(new { isValid = isValid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, new { message = "Internal server error during token validation" });
            }
        }

        [HttpGet("user-info")]
        public async Task<IActionResult> GetUserInfo([FromQuery] string accessToken)
        {
            try
            {
                _logger.LogInformation("Received user info request");

                if (string.IsNullOrEmpty(accessToken))
                {
                    return BadRequest(new { message = "Access token is required" });
                }

                var userInfo = await _keycloakService.GetUserInfoAsync(accessToken);
                
                if (userInfo != null)
                {
                    _logger.LogInformation("User info retrieved successfully for subject: {Subject}", userInfo.Sub);
                    return Ok(userInfo);
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve user info");
                    return BadRequest(new { message = "Failed to retrieve user info" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user info");
                return StatusCode(500, new { message = "Internal server error during user info retrieval" });
            }
        }

        [HttpPost("validate-session-state")]
        public async Task<IActionResult> ValidateSessionState([FromBody] SessionStateValidationRequest request)
        {
            try
            {
                _logger.LogInformation("Received session state validation request");

                if (string.IsNullOrEmpty(request.SessionState) || string.IsNullOrEmpty(request.Code))
                {
                    return BadRequest(new { message = "Session state and code are required" });
                }

                var result = await _keycloakService.ValidateSessionStateAsync(request.SessionState, request.Code);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating session state");
                return StatusCode(500, new { message = "Internal server error during session state validation" });
            }
        }

        [HttpGet("debug-config")]
        public IActionResult GetDebugConfig()
        {
            try
            {
                var config = new
                {
                    BaseUrl = "http://localhost:8080",
                    Realm = "Okta-Broker",
                    ClientId = "okta-client",
                    RedirectUri = "http://localhost:3000/callback",
                    TokenUrl = "http://localhost:8080/realms/Okta-Broker/protocol/openid-connect/token"
                };
                
                return Ok(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting debug config");
                return StatusCode(500, new { message = "Error getting debug config" });
            }
        }
    }

    public class TokenValidationRequest
    {
        public string AccessToken { get; set; } = string.Empty;
    }

    public class SessionStateValidationRequest
    {
        public string SessionState { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}

