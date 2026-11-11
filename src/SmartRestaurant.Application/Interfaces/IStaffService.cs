namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IStaffService
{
    Task<List<StaffMemberDto>> GetStaffMembersAsync(Guid branchId, CancellationToken ct = default);
    Task<StaffMemberDto?> GetStaffByIdAsync(Guid staffId, CancellationToken ct = default);
    Task<StaffMemberDto> CreateStaffAsync(CreateStaffRequest request, CancellationToken ct = default);
    Task<StaffMemberDto> UpdateStaffAsync(UpdateStaffRequest request, CancellationToken ct = default);
    Task<bool> ResetPinAsync(ResetPinRequest request, CancellationToken ct = default);
    Task<bool> VerifyPinAsync(Guid staffId, string pinCode, CancellationToken ct = default);
    Task<bool> AuthorizeManagerPinAsync(Guid branchId, string pinCode, string actionName, string targetEntity, string reason, CancellationToken ct = default);
    Task<bool> ToggleStaffStatusAsync(Guid staffId, CancellationToken ct = default);
}
