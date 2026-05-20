using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
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
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser([FromBody] LoginRequestDTO userData)
        {
            string token = await _userService.LoginUserAsync(userData);

            Response.Cookies.Append("access_token", token, 
            new CookieOptions 
            { 
                HttpOnly = true, 
                SameSite = SameSiteMode.Lax, 
                Secure = false, 
                Expires = DateTime.UtcNow.AddHours(2) 
            });

            return Ok();
        } 

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDTO userData)
        {
            await _userService.RegisterUserAsync(userData);     
                        
            return Ok();
        }
    }
}
