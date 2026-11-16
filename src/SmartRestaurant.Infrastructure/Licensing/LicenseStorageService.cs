namespace SmartRestaurant.Infrastructure.Licensing;

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class LicenseStorageService : ILicenseStorageService
{
    private static readonly byte[] MetadataEntropy = "SmartRestaurant-Meta-Entropy-v1"u8.ToArray();
    private readonly LicenseOptions _options;
    private readonly ILogger<LicenseStorageService> _logger;
    private readonly string _licenseFilePath;
    private readonly string _metadataFilePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public LicenseStorageService(
        IOptions<LicenseOptions> options,
        ILogger<LicenseStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;

        var baseDir = string.IsNullOrWhiteSpace(_options.StorageDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SmartRestaurant", "Licensing")
            : _options.StorageDirectory;

        Directory.CreateDirectory(baseDir);
        _licenseFilePath = Path.Combine(baseDir, "active_license.lic");
        _metadataFilePath = Path.Combine(baseDir, "license_state.dat");
    }

    public async Task<StoredLicenseDocument?> LoadCurrentLicenseAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_licenseFilePath))
            {
                return null;
            }

            var content = await File.ReadAllTextAsync(_licenseFilePath, Encoding.UTF8, cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            return ParseLicenseEnvelope(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đọc giấy phép bản quyền từ tệp lưu trữ.");
            return null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveLicenseAsync(StoredLicenseDocument document, CancellationToken cancellationToken = default)
    {
        if (document == null || string.IsNullOrWhiteSpace(document.PayloadJson) || string.IsNullOrWhiteSpace(document.SignatureBase64))
        {
            throw new ArgumentException("Dữ liệu giấy phép lưu trữ không hợp lệ.", nameof(document));
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var serializedEnvelope = PackageLicenseEnvelope(document.PayloadJson, document.SignatureBase64);

            // Ghi nguyên tử (Atomic write: ghi ra tệp tạm rồi hoán đổi)
            // Nhằm đảm bảo an toàn tuyệt đối nếu tiến trình bị ngắt đột ngột
            var tempFile = _licenseFilePath + ".tmp." + Guid.NewGuid().ToString("N");
            await File.WriteAllTextAsync(tempFile, serializedEnvelope, Encoding.UTF8, cancellationToken);

            File.Move(tempFile, _licenseFilePath, overwrite: true);
            _logger.LogInformation("Đã lưu trữ an toàn giấy phép bản quyền {LicenseId}.", document.License?.LicenseId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<LicenseStateMetadata> LoadStateMetadataAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_metadataFilePath))
            {
                return new LicenseStateMetadata { LastKnownTrustedUtc = DateTime.UtcNow };
            }

            var rawBytes = await File.ReadAllBytesAsync(_metadataFilePath, cancellationToken);
            byte[] plainBytes;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    plainBytes = ProtectedData.Unprotect(rawBytes, MetadataEntropy, DataProtectionScope.LocalMachine);
                }
                catch
                {
                    plainBytes = rawBytes;
                }
            }
            else
            {
                plainBytes = rawBytes;
            }

            var json = Encoding.UTF8.GetString(plainBytes);
            return JsonSerializer.Deserialize<LicenseStateMetadata>(json) ?? new LicenseStateMetadata { LastKnownTrustedUtc = DateTime.UtcNow };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không thể đọc siêu dữ liệu kiểm thực; khởi tạo mặc định.");
            return new LicenseStateMetadata { LastKnownTrustedUtc = DateTime.UtcNow };
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveStateMetadataAsync(LicenseStateMetadata metadata, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = false });
            var plainBytes = Encoding.UTF8.GetBytes(json);
            byte[] dataToSave;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                dataToSave = ProtectedData.Protect(plainBytes, MetadataEntropy, DataProtectionScope.LocalMachine);
            }
            else
            {
                dataToSave = plainBytes;
            }

            var tempFile = _metadataFilePath + ".tmp." + Guid.NewGuid().ToString("N");
            await File.WriteAllBytesAsync(tempFile, dataToSave, cancellationToken);
            File.Move(tempFile, _metadataFilePath, overwrite: true);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public static StoredLicenseDocument? ParseLicenseEnvelope(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        var cleaned = content.Trim();
        const string header = "-----BEGIN SMARTRESTAURANT LICENSE-----";
        const string footer = "-----END SMARTRESTAURANT LICENSE-----";

        if (cleaned.Contains(header, StringComparison.OrdinalIgnoreCase))
        {
            var start = cleaned.IndexOf(header, StringComparison.OrdinalIgnoreCase) + header.Length;
            var end = cleaned.IndexOf(footer, StringComparison.OrdinalIgnoreCase);
            cleaned = (end > start ? cleaned[start..end] : cleaned[start..]).Trim();
        }

        if (cleaned.Contains('.'))
        {
            var parts = cleaned.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var payloadBytes = Convert.FromBase64String(parts[0].Trim());
                var payloadJson = Encoding.UTF8.GetString(payloadBytes);
                var signatureBase64 = parts[1].Trim();

                var licenseModel = JsonSerializer.Deserialize<LicenseModel>(payloadJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (licenseModel != null)
                {
                    return new StoredLicenseDocument
                    {
                        RawDocument = content,
                        PayloadJson = payloadJson,
                        SignatureBase64 = signatureBase64,
                        License = licenseModel,
                        InstalledAt = DateTime.UtcNow
                    };
                }
            }
        }

        return null;
    }

    public static string PackageLicenseEnvelope(string payloadJson, string signatureBase64)
    {
        var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson.Trim()));
        var token = $"{payloadBase64}.{signatureBase64.Trim()}";

        var sb = new StringBuilder();
        sb.AppendLine("-----BEGIN SMARTRESTAURANT LICENSE-----");
        sb.AppendLine(token);
        sb.AppendLine("-----END SMARTRESTAURANT LICENSE-----");

        return sb.ToString();
    }
}
