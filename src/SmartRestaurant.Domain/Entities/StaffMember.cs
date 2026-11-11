namespace SmartRestaurant.Domain.Entities;

using System.Security.Cryptography;
using System.Text;
using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class StaffMember : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public StaffRole Role { get; private set; }
    public string PinHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    protected StaffMember() { }

    public StaffMember(Guid branchId, string fullName, string username, string email, StaffRole role, string pinCode = "1234")
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainValidationException("Họ và tên nhân viên không được để trống.");
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainValidationException("Tên đăng nhập không được để trống.");

        BranchId = branchId;
        FullName = fullName.Trim();
        Username = username.Trim().ToLowerInvariant();
        Email = email?.Trim() ?? string.Empty;
        Role = role;
        SetPin(pinCode);
        IsActive = true;
    }

    public void UpdateProfile(string fullName, string email, StaffRole role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainValidationException("Họ và tên nhân viên không được để trống.");

        FullName = fullName.Trim();
        Email = email?.Trim() ?? string.Empty;
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPin(string pinCode)
    {
        if (string.IsNullOrWhiteSpace(pinCode) || pinCode.Length != 4 || !pinCode.All(char.IsDigit))
            throw new DomainValidationException("Mã PIN phải gồm đúng 4 chữ số.");

        PinHash = HashPin(pinCode);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool VerifyPin(string pinCode)
    {
        if (string.IsNullOrWhiteSpace(pinCode) || string.IsNullOrEmpty(PinHash))
            return false;

        return PinHash == HashPin(pinCode);
    }

    public static string HashPin(string pinCode)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(pinCode.Trim()));
        return Convert.ToHexString(bytes);
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
