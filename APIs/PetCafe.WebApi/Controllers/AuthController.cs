
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.AuthModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

public class AuthController(IAuthService _authService) : BaseController
{

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] AuthRequestModel model)
    {
        return Ok(await _authService.SignInAsync(model));
    }

    [HttpPost("revoke")]
    public IActionResult Revoke([FromBody] RevokeModel model)
    {
        var result = _authService.Revoke(model);
        return Ok(result);
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] AuthGoogleRequestModel model)
    {
        return Ok(await _authService.SignWithGoogleAsync(model));
    }


}