namespace PetCafe.Application.GlobalExceptionHandling.Exceptions
{
    public class KeyNotFoundException(string? message) : Exception(message)
    {
    }
}
