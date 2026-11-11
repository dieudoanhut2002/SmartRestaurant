namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class RestaurantTable : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public Guid DiningAreaId { get; private set; }
    public Guid? DiningRoomId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public TableStatus Status { get; private set; } = TableStatus.Available;
    public TableShape Shape { get; private set; } = TableShape.Rectangle;

    // Visual floor plan layout coordinates (Konva.js canvas)
    public double PositionX { get; private set; }
    public double PositionY { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double Rotation { get; private set; }
    public bool IsActive { get; private set; } = true;

    protected RestaurantTable() { }

    public RestaurantTable(
        Guid branchId,
        Guid diningAreaId,
        string code,
        string name,
        int capacity,
        Guid? diningRoomId = null,
        TableShape shape = TableShape.Rectangle,
        double posX = 0,
        double posY = 0,
        double width = 80,
        double height = 80)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (diningAreaId == Guid.Empty)
            throw new DomainValidationException("DiningAreaId không được để trống.");
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainValidationException("Mã bàn không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên bàn không được để trống.");
        if (capacity <= 0)
            throw new DomainValidationException("Sức chứa của bàn phải lớn hơn 0.");

        BranchId = branchId;
        DiningAreaId = diningAreaId;
        DiningRoomId = diningRoomId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Capacity = capacity;
        Status = TableStatus.Available;
        Shape = shape;
        PositionX = posX;
        PositionY = posY;
        Width = width > 0 ? width : 80;
        Height = height > 0 ? height : 80;
        Rotation = 0;
        IsActive = true;
    }

    public void Occupy()
    {
        if (Status != TableStatus.Available && Status != TableStatus.Reserved)
            throw new InvalidTableStateTransitionException(Code, Status, TableStatus.Occupied);

        Status = TableStatus.Occupied;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RequestBilling()
    {
        if (Status != TableStatus.Occupied)
            throw new InvalidTableStateTransitionException(Code, Status, TableStatus.BillingRequested);

        Status = TableStatus.BillingRequested;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReleaseToCleaning()
    {
        if (Status != TableStatus.Occupied && Status != TableStatus.BillingRequested)
            throw new InvalidTableStateTransitionException(Code, Status, TableStatus.CleaningNeeded);

        Status = TableStatus.CleaningNeeded;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteCleaning()
    {
        if (Status != TableStatus.CleaningNeeded)
            throw new InvalidTableStateTransitionException(Code, Status, TableStatus.Available);

        Status = TableStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reserve()
    {
        if (Status != TableStatus.Available)
            throw new InvalidTableStateTransitionException(Code, Status, TableStatus.Reserved);

        Status = TableStatus.Reserved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelReservation()
    {
        if (Status != TableStatus.Reserved)
            throw new InvalidTableStateTransitionException(Code, Status, TableStatus.Available);

        Status = TableStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLayout(double posX, double posY, double width, double height, double rotation)
    {
        PositionX = posX;
        PositionY = posY;
        Width = width > 0 ? width : Width;
        Height = height > 0 ? height : Height;
        Rotation = rotation;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string name, int capacity, TableShape shape)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên bàn không được để trống.");
        if (capacity <= 0)
            throw new DomainValidationException("Sức chứa của bàn phải lớn hơn 0.");

        Name = name.Trim();
        Capacity = capacity;
        Shape = shape;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
