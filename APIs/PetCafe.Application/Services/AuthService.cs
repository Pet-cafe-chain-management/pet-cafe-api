using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.AuthModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IAuthService
{
    Task<AuthResponseModel> SignInAsync(AuthRequestModel model);
    string Revoke(RevokeModel revoke);
    Task<AuthResponseModel> SignWithGoogleAsync(AuthGoogleRequestModel model);

}


public class AuthService(
    IHashService _hashService,
    IUnitOfWork _unitOfWork,
    AppSettings _appSettings
) : IAuthService
{
    public string Revoke(RevokeModel revoke)
    {
        return TokenGenerator.RevokeToken(revoke, _appSettings);
    }

    public async Task<AuthResponseModel> SignInAsync(AuthRequestModel model)
    {
        var account = await _unitOfWork.AccountRepository
            .FirstOrDefaultAsync(x =>
                x.Email == model.Email && x.IsActive, includeFunc: x => x.Include(x => x.Employee!).Include(x => x.Customer!)
            ) ?? throw new BadRequestException("Tài khoản không hợp lệ!");

        if (!_hashService.VerifyPassword(model.Password, account.PasswordHash)) throw new BadRequestException("Tài khoản không hợp lệ!");

        return new AuthResponseModel
        {
            AccessToken = TokenGenerator.GenerateToken(account, _appSettings),
            RefreshToken = TokenGenerator.GenerateToken(account, _appSettings),
            Account = account
        };

    }

    private async Task<Account> CreateAccountAsync(User userGoogleProfile)
    {

        var account = await _unitOfWork.AccountRepository.AddAsync(new Account
        {
            Email = userGoogleProfile.Email,
            Username = userGoogleProfile.DisplayName,
            Role = RoleConstants.CUSTOMER,
            PasswordHash = _hashService.GenerateRandomPassword()
        });

        account.Customer = await _unitOfWork.CustomerRepository.AddAsync(new Customer
        {
            AccountId = account.Id,
            FullName = userGoogleProfile.DisplayName,
            Email = userGoogleProfile.Email,
            AvatarUrl = userGoogleProfile.PhotoUrl ?? string.Empty
        });

        await _unitOfWork.SaveChangesAsync();

        return account;
    }
    public async Task<AuthResponseModel> SignWithGoogleAsync(AuthGoogleRequestModel model)
    {
        var authGoogle = new FirebaseAuthProvider(new FirebaseConfig(apiKey: _appSettings.FirebaseSettings.ApiKeY));
        var userGoogleProfile = await authGoogle.GetUserAsync(model.AccessToken)
            ?? throw new BadRequestException($"Tài khoản không tồn tại!");

        var account = await _unitOfWork.AccountRepository
            .FirstOrDefaultAsync(x => x.Email == userGoogleProfile.Email && x.IsActive, includeFunc: x => x.Include(x => x.Customer!).Include(x => x.Employee!)) ?? await CreateAccountAsync(userGoogleProfile);

        return new AuthResponseModel
        {
            AccessToken = TokenGenerator.GenerateToken(account, _appSettings),
            RefreshToken = TokenGenerator.GenerateToken(account, _appSettings),
            Account = account
        };
    }
}
