namespace PetCafe.Application.GlobalExceptionHandling.Exceptions
{
    public class UnauthorizedAccessException(string? message) : Exception(message)
    {
    }
}
