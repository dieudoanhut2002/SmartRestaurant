namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IGuestOrderService
{
    Task<GuestTableSessionDto> GetTableSessionAsync(Guid tableId, string? token, CancellationToken ct = default);
    Task<GuestOrderResultDto> SubmitGuestOrderAsync(GuestOrderRequestDto request, CancellationToken ct = default);
    Task<bool> RequestBillAsync(Guid tableId, string? token, CancellationToken ct = default);
    Task<TableQrInfoDto> GetTableQrInfoAsync(Guid tableId, string baseUrl, CancellationToken ct = default);
}
