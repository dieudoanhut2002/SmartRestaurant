namespace SmartRestaurant.Application.Interfaces;

public interface ITableQrTokenService
{
    string GenerateToken(Guid tableId);
    bool ValidateToken(Guid tableId, string? token);
    string BuildGuestOrderUrl(string baseUrl, Guid tableId);
}
