namespace SmartRestaurant.Application.Tests;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Infrastructure.Licensing;

/// <summary>
/// Công cụ ký số giấy phép bản quyền DÀNH RIÊNG CHO MÔI TRƯỜNG KIỂM THỬ (Test-Only Tooling)
/// Khóa riêng tư này CHỈ tồn tại trong dự án Unit/Integration Test, TUYỆT ĐỐI không có trong mã nguồn hoặc binary phát hành.
/// </summary>
public static class TestLicenseSigner
{
    // Cặp khóa ECDSA P-256 dùng riêng cho test
    public const string TestIssuerPublicKeyPem = """
-----BEGIN PUBLIC KEY-----
MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAENgUaeS36Tny5wOaisakCvoA/f9D9
unt9VuY2yHEXalxz1MZYerZcvTGYTvAHMCBYJPVyt2jURgy60Q9GCYmxjQ==
-----END PUBLIC KEY-----
""";

    public const string TestIssuerPrivateKeyPem = """
-----BEGIN PRIVATE KEY-----
MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgcu2DWSWi7imzTNRI
gSKuLJ0X6iYHuVVB5ExlA0b0Q6ihRANCAAQ2BRp5LfpOfLnA5qKxqQK+gD9/0P26
e31W5jbIcRdqXHPUxlh6tly9MZhO8AcwIFgk9XK3aNRGDLrRD0YJibGN
-----END PRIVATE KEY-----
""";

    public static string CreateSignedLicenseDocument(LicenseModel model, string? privateKeyPem = null)
    {
        var keyToUse = privateKeyPem ?? TestIssuerPrivateKeyPem;

        var payloadJson = JsonSerializer.Serialize(model, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(keyToUse);

        var data = Encoding.UTF8.GetBytes(payloadJson.Trim());
        var signatureBytes = ecdsa.SignData(data, HashAlgorithmName.SHA256);
        var signatureBase64 = Convert.ToBase64String(signatureBytes);

        return LicenseStorageService.PackageLicenseEnvelope(payloadJson, signatureBase64);
    }
}
