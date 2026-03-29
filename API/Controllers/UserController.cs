using Microsoft.AspNetCore.Mvc;
using myfinance.Application.Services.Interfaces;
using myfinance.Domain.DTOS;

namespace myfinance.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController(IUserService userService, ITokenService tokenService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDTO userData)
        {
            var teste = _tokenService.GenerateJWT(1);

            return Ok(userData);
        }
    }
}
