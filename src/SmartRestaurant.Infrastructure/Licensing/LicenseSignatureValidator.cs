namespace SmartRestaurant.Infrastructure.Licensing;

using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class LicenseSignatureValidator(
    IOptions<LicenseOptions> options,
    ILogger<LicenseSignatureValidator> logger) : ILicenseSignatureValidator
{
    private readonly LicenseOptions _options = options.Value;
    private readonly ILogger<LicenseSignatureValidator> _logger = logger;

    public bool VerifySignature(string serializedPayload, string signatureBase64)
    {
        if (string.IsNullOrWhiteSpace(serializedPayload) || string.IsNullOrWhiteSpace(signatureBase64))
        {
            return false;
        }

        var publicKeyPem = _options.IssuerPublicKeyPem;
        if (string.IsNullOrWhiteSpace(publicKeyPem))
        {
            _logger.LogWarning("Khóa công khai kiểm thực bản quyền (IssuerPublicKeyPem) chưa được cấu hình.");
            return false;
        }

        try
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(publicKeyPem.Trim());

            var payloadBytes = Encoding.UTF8.GetBytes(serializedPayload.Trim());
            var signatureBytes = Convert.FromBase64String(signatureBase64.Trim());

            return ecdsa.VerifyData(payloadBytes, signatureBytes, HashAlgorithmName.SHA256);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Lỗi kiểm thực chữ ký số ECDSA P-256.");
            return false;
        }
    }
}
