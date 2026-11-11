namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using Xunit;

public class StaffAndShiftServiceTests
{
    private readonly Guid _branchId = Guid.NewGuid();

    [Fact]
    public async Task CreateStaff_WithValidPin_ShouldHashPinAndSave()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(CreateStaff_WithValidPin_ShouldHashPinAndSave));
        var service = new StaffService(context);

        var request = new CreateStaffRequest(_branchId, "Nguyễn Văn Test", "test.nv", "test@restaurant.vn", StaffRole.Cashier, "5678");

        // Act
        var result = await service.CreateStaffAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be("Nguyễn Văn Test");
        result.Role.Should().Be(StaffRole.Cashier);

        var memberInDb = await context.StaffMembers.FirstOrDefaultAsync(s => s.Id == result.Id);
        memberInDb!.PinHash.Should().NotBe("5678"); // Securely hashed
        memberInDb.VerifyPin("5678").Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeManagerPin_WithValidManagerPin_ShouldSucceedAndCreateAuditLog()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(AuthorizeManagerPin_WithValidManagerPin_ShouldSucceedAndCreateAuditLog));
        var manager = new StaffMember(_branchId, "Quản Lý Trần", "manager.tran", "mgr@restaurant.vn", StaffRole.RestaurantManager, "9876");
        context.StaffMembers.Add(manager);
        await context.SaveChangesAsync();

        var service = new StaffService(context);

        // Act
        var authorized = await service.AuthorizeManagerPinAsync(
            _branchId,
            "9876",
            "Hủy món nấu sau khi xào",
            "Bàn T01 / Món Bò Lúc Lắc",
            "Khách bị dị ứng hành tây");

        // Assert
        authorized.Should().BeTrue();

        var auditLog = await context.ManagerPinAuditLogs.FirstOrDefaultAsync();
        auditLog.Should().NotBeNull();
        auditLog!.ManagerStaffId.Should().Be(manager.Id);
        auditLog.ActionName.Should().Be("Hủy món nấu sau khi xào");
        auditLog.Reason.Should().Be("Khách bị dị ứng hành tây");
    }

    [Fact]
    public async Task AuthorizeManagerPin_WithWaiterPin_ShouldFail()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(AuthorizeManagerPin_WithWaiterPin_ShouldFail));
        var waiter = new StaffMember(_branchId, "Bồi Bàn Lê", "waiter.le", "waiter@restaurant.vn", StaffRole.Waiter, "1111");
        context.StaffMembers.Add(waiter);
        await context.SaveChangesAsync();

        var service = new StaffService(context);

        // Act: Attempt to authorize using Waiter's PIN
        var authorized = await service.AuthorizeManagerPinAsync(
            _branchId,
            "1111",
            "Chiết khấu hóa đơn 50%",
            "Bàn T02",
            "Yêu cầu giảm giá");

        // Assert
        authorized.Should().BeFalse();
        (await context.ManagerPinAuditLogs.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task CloseShiftBlind_ShouldCalculateVarianceAndLog()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(CloseShiftBlind_ShouldCalculateVarianceAndLog));
        var cashier = new StaffMember(_branchId, "Thu Ngân Mai", "cashier.mai", "mai@restaurant.vn", StaffRole.Cashier, "2222");
        context.StaffMembers.Add(cashier);

        var shift = new WorkShift(_branchId, cashier.Id, 2000000m, "Ca sáng");
        shift.AddSystemCash(5000000m); // Total expected = 2,000,000 float + 5,000,000 system = 7,000,000
        context.WorkShifts.Add(shift);
        await context.SaveChangesAsync();

        var service = new ShiftService(context);
        // Cashier blind count: 6,950,000 (Shortage of 50,000)
        var closeRequest = new CloseShiftBlindRequest(shift.Id, 6950000m, "Tiền thối bị thất thoát 50k");

        // Act
        var varianceLog = await service.CloseShiftBlindAsync(closeRequest);

        // Assert
        varianceLog.Should().NotBeNull();
        varianceLog.ExpectedAmount.Should().Be(7000000m);
        varianceLog.ActualCountedAmount.Should().Be(6950000m);
        varianceLog.VarianceAmount.Should().Be(-50000m); // Shortage recorded
        varianceLog.CashierNotes.Should().Be("Tiền thối bị thất thoát 50k");

        var updatedShift = await context.WorkShifts.FirstOrDefaultAsync(s => s.Id == shift.Id);
        updatedShift!.Status.Should().Be(ShiftStatus.Closed);
    }
}
