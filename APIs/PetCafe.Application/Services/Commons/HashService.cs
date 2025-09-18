using PetCafe.Application.GlobalExceptionHandling.Exceptions;

namespace PetCafe.Application.Services.Commons;


public interface IHashService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);

    string GenerateRandomPassword(int length = 12);
}

public class HashService(int workFactor = 12) : IHashService
{
    private readonly int _workFactor = workFactor;

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new BadRequestException("Tài khoản không hợp lệ!");

        return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }

    public string GenerateRandomPassword(int length = 12)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
        var random = new Random();
        var password = new char[length];

        for (int i = 0; i < length; i++)
        {
            password[i] = validChars[random.Next(validChars.Length)];
        }

        return new string(password);
    }
}