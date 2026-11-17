namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class KitchenTicket : BaseEntity
{
    public Guid KitchenStationId { get; private set; }
    public Guid OrderRoundId { get; private set; }
    public string TicketNumber { get; private set; } = string.Empty;
    public string TableCode { get; private set; } = string.Empty;
    public bool IsHighPriority { get; private set; }
    public KitchenTicketStatus Status { get; private set; } = KitchenTicketStatus.Pending;
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }
    public string ItemsJson { get; private set; } = string.Empty;

    protected KitchenTicket() { }

    public KitchenTicket(
        Guid kitchenStationId,
        Guid orderRoundId,
        string ticketNumber,
        string itemsJson,
        string? notes = null,
        string tableCode = "",
        bool isHighPriority = false)
    {
        if (kitchenStationId == Guid.Empty)
            throw new DomainValidationException("KitchenStationId không được để trống.");
        if (orderRoundId == Guid.Empty)
            throw new DomainValidationException("OrderRoundId không được để trống.");
        if (string.IsNullOrWhiteSpace(ticketNumber))
            throw new DomainValidationException("Số phiếu bếp không được để trống.");

        KitchenStationId = kitchenStationId;
        OrderRoundId = orderRoundId;
        TicketNumber = ticketNumber.Trim();
        ItemsJson = itemsJson ?? "[]";
        Notes = notes?.Trim();
        TableCode = tableCode?.Trim() ?? string.Empty;
        IsHighPriority = isHighPriority;
        Status = KitchenTicketStatus.Pending;
    }

    public void MarkAsHighPriority(string reason)
    {
        IsHighPriority = true;
        Notes = string.IsNullOrWhiteSpace(Notes) ? $"[ƯU TIÊN LÀM LẠI] {reason}" : $"{Notes} | [ƯU TIÊN LÀM LẠI] {reason}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(KitchenTicketStatus newStatus)
    {
        switch (newStatus)
        {
            case KitchenTicketStatus.InPreparation:
                StartPreparation();
                break;
            case KitchenTicketStatus.Completed:
                Complete();
                break;
            case KitchenTicketStatus.Cancelled:
                Cancel();
                break;
            default:
                Status = newStatus;
                UpdatedAt = DateTime.UtcNow;
                break;
        }
    }

    public void StartPreparation()
    {
        if (Status != KitchenTicketStatus.Pending)
            throw new DomainValidationException($"Không thể bắt đầu chế biến khi phiếu ở trạng thái {Status}.");

        Status = KitchenTicketStatus.InPreparation;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = KitchenTicketStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = KitchenTicketStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
