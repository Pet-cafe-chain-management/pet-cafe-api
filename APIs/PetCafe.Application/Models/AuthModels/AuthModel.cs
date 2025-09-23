using System.ComponentModel.DataAnnotations;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Models.AuthModels;

public class AuthGoogleRequestModel
{
    [Required(ErrorMessage = "Access token is required")]
    public string AccessToken { get; set; } = default!;
}

public class AuthRequestModel
{
    [Required(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Mật khẩu không hợp lệ")]
    public string Password { get; set; } = default!;

    public string asnbnn { get; set; } = default!;
}

public class AuthResponseModel
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public required Account Account { get; set; }
}

public class RevokeModel
{
    [Required(ErrorMessage = "RefreshToken is required")]
    public string RefreshToken { get; set; } = default!;
}