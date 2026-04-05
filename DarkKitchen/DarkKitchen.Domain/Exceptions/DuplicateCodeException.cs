namespace DarkKitchen.Domain.Exceptions;

public class DuplicateCodeException(string message) : Exception(message)
{
}
