namespace SmartRestaurant.Application.Services;

using System.Security.Cryptography;
using System.Text;
using SmartRestaurant.Application.Interfaces;

public class TableQrTokenService : ITableQrTokenService
{
    private readonly byte[] _secretKeyBytes;

    public TableQrTokenService(string? secretKey = null)
    {
        var key = string.IsNullOrWhiteSpace(secretKey)
            ? "SmartRestaurant-Guest-Secret-v1-Salt-Key-2026-Enterprise-QR"
            : secretKey.Trim();
        _secretKeyBytes = Encoding.UTF8.GetBytes(key);
    }

    public string GenerateToken(Guid tableId)
    {
        using var hmac = new HMACSHA256(_secretKeyBytes);
        var payloadBytes = Encoding.UTF8.GetBytes(tableId.ToString("N"));
        var hash = hmac.ComputeHash(payloadBytes);
        return Convert.ToBase64String(hash)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public bool ValidateToken(Guid tableId, string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var expectedToken = GenerateToken(tableId);

        var tokenBytes = Encoding.UTF8.GetBytes(token.Trim());
        var expectedBytes = Encoding.UTF8.GetBytes(expectedToken);

        if (tokenBytes.Length != expectedBytes.Length)
            return false;

        return CryptographicOperations.FixedTimeEquals(tokenBytes, expectedBytes);
    }

    public string BuildGuestOrderUrl(string baseUrl, Guid tableId)
    {
        var token = GenerateToken(tableId);
        var trimmedBase = (baseUrl ?? string.Empty).TrimEnd('/');
        return $"{trimmedBase}/Guest?tableId={tableId}&token={token}";
    }
}
