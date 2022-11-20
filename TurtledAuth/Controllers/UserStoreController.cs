using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TurtledAuth.Models.AuthenticationResources.Communication;
using TurtledAuth.Services;
using TurtledDictionary.Resources.Authentication;

namespace TurtledAuth.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserStoreController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthenticationJWTService _jwtService;

        public UserStoreController(UserManager<IdentityUser> userManager, AuthenticationJWTService service)
        {
            _userManager = userManager;
            _jwtService = service;
        }

        [HttpPost]
        public async Task<ActionResult<IdentityUser>> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _userManager.CreateAsync(new IdentityUser { UserName = user.UserName, Email = user.Email}, user.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            user.Password = string.Empty;
            return Created("", user);
        }

        [HttpPost("token")]
        public async Task<ActionResult<AuthenticationJWTResponse>> GetBearerToken(AuthenticationJWTRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            IdentityUser user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return BadRequest("Bad credentials");
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isValidPassword)
            {
                return BadRequest("Bad credentials");
            }

            var token = _jwtService.GetToken(user);

            return Ok(token);
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<AuthenticationJWTResponse>> RefreshToken(TokenPair tokenPair)
        {
            var principal = _jwtService.GetPrincipalFromToken(tokenPair.Token);
            IdentityUser user = await _userManager.FindByNameAsync(principal?.Identity?.Name);
            
            if(user == null)
            {
                return NotFound();
            }

            return Ok(_jwtService.GetValidateRefreshToken(user, tokenPair.RefreshToken, principal));
        }

        [HttpGet("success")]
        [Authorize]
        public IActionResult BlankSuccess() 
        { 
            return Ok();
        }

        [HttpGet("account")]
        [Authorize(Policy = "OwnerRestricted")]
        public async Task<IActionResult> ClaimCorrect()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response;

            using(var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5000/payment"))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Headers.Authorization);

                response = await client.SendAsync(requestMessage);
            }
            return Ok(response.StatusCode);
        }


    }
}
