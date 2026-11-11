namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class DiningRoom : BaseEntity
{
    public Guid DiningAreaId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public bool IsVip { get; private set; }
    public decimal? SurchargeRate { get; private set; }
    public decimal? SurchargeFixedAmount { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<RestaurantTable> _tables = [];
    public IReadOnlyCollection<RestaurantTable> Tables => _tables.AsReadOnly();

    protected DiningRoom() { }

    public DiningRoom(Guid diningAreaId, string name, bool isVip = false, decimal? surchargeRate = null, decimal? surchargeFixedAmount = null)
    {
        if (diningAreaId == Guid.Empty)
            throw new DomainValidationException("DiningAreaId không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên phòng không được để trống.");

        DiningAreaId = diningAreaId;
        Name = name.Trim();
        IsVip = isVip;
        SurchargeRate = surchargeRate;
        SurchargeFixedAmount = surchargeFixedAmount;
        IsActive = true;
    }

    public void UpdateDetails(string name, bool isVip, decimal? surchargeRate, decimal? surchargeFixedAmount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên phòng không được để trống.");

        Name = name.Trim();
        IsVip = isVip;
        SurchargeRate = surchargeRate;
        SurchargeFixedAmount = surchargeFixedAmount;
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
