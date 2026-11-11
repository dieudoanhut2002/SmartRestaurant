namespace SmartRestaurant.Domain.Exceptions;

public class DomainNotFoundException(string message) : DomainException(message)
{
}
