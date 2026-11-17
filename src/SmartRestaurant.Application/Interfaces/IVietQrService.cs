namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IVietQrService
{
    VietQrPayloadDto GenerateDynamicVietQr(
        string invoiceNumber,
        decimal amount,
        string? bankBin = null,
        string? accountNumber = null,
        string? accountName = null);

    bool ValidateCrc16(string emvCoPayload);
}
