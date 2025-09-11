namespace PetCafe.Application.GlobalExceptionHandling.Exceptions
{
    public class NotFoundException(string? message) : Exception(message)
    {
    }
}
