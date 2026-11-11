namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class ModifierGroup : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int MinSelections { get; private set; }
    public int MaxSelections { get; private set; }
    public bool IsRequired => MinSelections > 0;
    public bool IsActive { get; private set; } = true;

    private readonly List<ModifierOption> _options = [];
    public IReadOnlyCollection<ModifierOption> Options => _options.AsReadOnly();

    protected ModifierGroup() { }

    public ModifierGroup(Guid branchId, string name, int minSelections = 0, int maxSelections = 1)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên nhóm tùy chọn không được để trống.");
        if (minSelections < 0 || maxSelections < minSelections)
            throw new DomainValidationException("Số lượng lựa chọn không hợp lệ.");

        BranchId = branchId;
        Name = name.Trim();
        MinSelections = minSelections;
        MaxSelections = maxSelections;
        IsActive = true;
    }

    public void Update(string name, int minSelections, int maxSelections)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên nhóm tùy chọn không được để trống.");
        if (minSelections < 0 || maxSelections < minSelections)
            throw new DomainValidationException("Số lượng lựa chọn không hợp lệ.");

        Name = name.Trim();
        MinSelections = minSelections;
        MaxSelections = maxSelections;
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
