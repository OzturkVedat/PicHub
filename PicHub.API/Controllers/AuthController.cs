using Microsoft.AspNetCore.Mvc;
using PicHub.API.Models;
using PicHub.API.Services;

namespace PicHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CognitoService _cognitoService;

        public AuthController(CognitoService cognitoService)
        {
            _cognitoService = cognitoService;
        }

        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _cognitoService.RegisterUser(request.Email, request.Password);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("confirm-account")]
        public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmRequest request)
        {
            var confirmResult = await _cognitoService.ConfirmUser(request);
            return confirmResult.IsSuccess ? Ok(confirmResult) : BadRequest(confirmResult);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var tokenResult = await _cognitoService.LoginUser(request.Email, request.Password);
            return tokenResult.IsSuccess ? Ok(tokenResult) : BadRequest(tokenResult);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            var newJwtResult = await _cognitoService.RefreshToken(request.RefreshToken);
            return newJwtResult.IsSuccess ? Ok(newJwtResult) : BadRequest(newJwtResult);
        }

    }
}
