namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using SmartRestaurant.Application.Services;
using Xunit;

public class VietQrServiceTests
{
    [Fact]
    public void GenerateDynamicVietQr_ShouldGenerateValidEmvCoPayload_AndCorrectCrc()
    {
        var service = new VietQrService();
        var result = service.GenerateDynamicVietQr(
            invoiceNumber: "INV-20261116-VIP01",
            amount: 250000m
        );

        result.Should().NotBeNull();
        result.Amount.Should().Be(250000);
        result.QrString.Should().StartWith("000201010212"); // Format + Dynamic QR flag
        result.QrString.Should().Contain("5303704");       // VND currency
        result.QrString.Should().Contain("5406250000");     // Amount 250000
        result.QrString.Should().Contain("5802VN");         // VN country
        result.QuickLink.Should().Contain("img.vietqr.io");
        result.QuickLink.Should().Contain("250000");

        // Validate CRC16
        bool isValidCrc = service.ValidateCrc16(result.QrString);
        isValidCrc.Should().BeTrue();
    }

    [Fact]
    public void ValidateCrc16_ShouldReturnFalse_WhenPayloadIsTampered()
    {
        var service = new VietQrService();
        var result = service.GenerateDynamicVietQr("INV-001", 100000m);

        // Tamper payload characters
        string tampered = result.QrString.Substring(0, result.QrString.Length - 5) + "X" + result.QrString.Substring(result.QrString.Length - 4);
        bool isValid = service.ValidateCrc16(tampered);

        isValid.Should().BeFalse();
    }
}
