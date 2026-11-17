namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class OutboxMessage : BaseEntity
{
    public string EventType { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = string.Empty;
    public DateTime? ProcessedAt { get; private set; }

    protected OutboxMessage() { }

    public OutboxMessage(string eventType, string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new DomainValidationException("EventType không được để trống.");

        EventType = eventType.Trim();
        PayloadJson = payloadJson ?? "{}";
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
