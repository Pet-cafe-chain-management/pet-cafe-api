namespace PetCafe.Application.GlobalExceptionHandling.Exceptions;

public class BadRequestException(string? message) : Exception(message)
{
}
