namespace SmartRestaurant.Infrastructure.Licensing;

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class InstallationIdentityService : IInstallationIdentityService
{
    private static readonly byte[] Entropy = "SmartRestaurant-Install-Entropy-v1"u8.ToArray();
    private readonly LicenseOptions _options;
    private readonly ILogger<InstallationIdentityService> _logger;
    private readonly string _identityFilePath;
    private InstallationIdentityDto? _cachedIdentity;
    private string? _cachedPrivateKeyPem;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public InstallationIdentityService(
        IOptions<LicenseOptions> options,
        ILogger<InstallationIdentityService> logger)
    {
        _options = options.Value;
        _logger = logger;

        var baseDir = string.IsNullOrWhiteSpace(_options.StorageDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SmartRestaurant", "Licensing")
            : _options.StorageDirectory;

        Directory.CreateDirectory(baseDir);
        _identityFilePath = Path.Combine(baseDir, "installation_identity.dat");
    }

    public async Task<InstallationIdentityDto> GetOrCreateInstallationIdentityAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedIdentity != null)
        {
            return _cachedIdentity;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_cachedIdentity != null)
            {
                return _cachedIdentity;
            }

            if (File.Exists(_identityFilePath))
            {
                try
                {
                    var rawBytes = await File.ReadAllBytesAsync(_identityFilePath, cancellationToken);
                    byte[] plainBytes;

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        plainBytes = ProtectedData.Unprotect(rawBytes, Entropy, DataProtectionScope.LocalMachine);
                    }
                    else
                    {
                        plainBytes = rawBytes;
                    }

                    var json = Encoding.UTF8.GetString(plainBytes);
                    var record = JsonSerializer.Deserialize<IdentityRecord>(json);

                    if (record != null && !string.IsNullOrWhiteSpace(record.InstallationId))
                    {
                        _cachedPrivateKeyPem = record.PrivateKeyPem;
                        _cachedIdentity = new InstallationIdentityDto
                        {
                            InstallationId = record.InstallationId,
                            HardwareFingerprint = await GetHardwareFingerprintAsync(cancellationToken),
                            MachineName = Environment.MachineName,
                            OperatingSystem = RuntimeInformation.OSDescription,
                            ProcessorCount = Environment.ProcessorCount,
                            PublicKeyPem = record.PublicKeyPem,
                            CreatedAtUtc = record.CreatedAtUtc
                        };

                        return _cachedIdentity;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Không thể đọc tệp định danh cài đặt hiện có; khởi tạo định danh mới.");
                }
            }

            // Sinh định danh ngẫu nhiên mới và cặp khóa máy chủ cục bộ
            var newInstallationId = Guid.NewGuid().ToString("D").ToUpperInvariant();
            using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var pubKeyPem = ecdsa.ExportSubjectPublicKeyInfoPem();
            var privKeyPem = ecdsa.ExportPkcs8PrivateKeyPem();
            var createdAt = DateTime.UtcNow;

            var newRecord = new IdentityRecord
            {
                InstallationId = newInstallationId,
                PublicKeyPem = pubKeyPem,
                PrivateKeyPem = privKeyPem,
                CreatedAtUtc = createdAt
            };

            var recordJson = JsonSerializer.Serialize(newRecord);
            var plainData = Encoding.UTF8.GetBytes(recordJson);
            byte[] dataToSave;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                dataToSave = ProtectedData.Protect(plainData, Entropy, DataProtectionScope.LocalMachine);
            }
            else
            {
                dataToSave = plainData;
            }

            // Ghi file an toàn
            var tempPath = _identityFilePath + ".tmp";
            await File.WriteAllBytesAsync(tempPath, dataToSave, cancellationToken);
            File.Move(tempPath, _identityFilePath, overwrite: true);

            _cachedPrivateKeyPem = privKeyPem;
            _cachedIdentity = new InstallationIdentityDto
            {
                InstallationId = newInstallationId,
                HardwareFingerprint = await GetHardwareFingerprintAsync(cancellationToken),
                MachineName = Environment.MachineName,
                OperatingSystem = RuntimeInformation.OSDescription,
                ProcessorCount = Environment.ProcessorCount,
                PublicKeyPem = pubKeyPem,
                CreatedAtUtc = createdAt
            };

            _logger.LogInformation("Đã khởi tạo thành công InstallationId: {InstallationId}", newInstallationId);
            return _cachedIdentity;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<string> GetHardwareFingerprintAsync(CancellationToken cancellationToken = default)
    {
        var rawComponents = new StringBuilder();

        // 1. Tên máy tính
        rawComponents.Append(Environment.MachineName.Trim().ToUpperInvariant()).Append('|');

        // 2. Hệ điều hành & Kiến trúc
        rawComponents.Append(RuntimeInformation.OSDescription.Trim()).Append('|');
        rawComponents.Append(RuntimeInformation.ProcessArchitecture).Append('|');

        // 3. Số lõi vi xử lý logic
        rawComponents.Append(Environment.ProcessorCount).Append('|');

        // 4. Windows Cryptography MachineGuid nếu có
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                using var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                var machineGuid = key?.GetValue("MachineGuid")?.ToString();
                if (!string.IsNullOrWhiteSpace(machineGuid))
                {
                    rawComponents.Append(machineGuid.Trim()).Append('|');
                }
            }
            catch
            {
                // Fallback nếu không có quyền đọc Registry
                rawComponents.Append(Environment.UserName).Append('|');
            }
        }
        else
        {
            rawComponents.Append(Environment.SystemDirectory).Append('|');
        }

        var inputBytes = Encoding.UTF8.GetBytes(rawComponents.ToString());
        var hashBytes = SHA256.HashData(inputBytes);
        var hex = Convert.ToHexString(hashBytes);

        var fingerprint = $"SR-FPRT-{hex[..4]}-{hex[4..8]}-{hex[8..12]}-{hex[12..16]}";
        return Task.FromResult(fingerprint);
    }

    public async Task<string> ExportRehostRequestAsync(CancellationToken cancellationToken = default)
    {
        var identity = await GetOrCreateInstallationIdentityAsync(cancellationToken);

        var rehostPayload = new
        {
            InstallationId = identity.InstallationId,
            HardwareFingerprint = identity.HardwareFingerprint,
            MachineName = identity.MachineName,
            TimestampUtc = DateTime.UtcNow,
            Action = "RehostLicenseRequest"
        };

        var payloadJson = JsonSerializer.Serialize(rehostPayload);

        string signatureBase64 = string.Empty;
        if (!string.IsNullOrWhiteSpace(_cachedPrivateKeyPem))
        {
            try
            {
                using var ecdsa = ECDsa.Create();
                ecdsa.ImportFromPem(_cachedPrivateKeyPem);
                var data = Encoding.UTF8.GetBytes(payloadJson);
                var sig = ecdsa.SignData(data, HashAlgorithmName.SHA256);
                signatureBase64 = Convert.ToBase64String(sig);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể ký yêu cầu rehost bằng khóa máy chủ cục bộ.");
            }
        }

        var envelope = new
        {
            Payload = rehostPayload,
            Signature = signatureBase64
        };

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope)));
    }

    private class IdentityRecord
    {
        public string InstallationId { get; set; } = string.Empty;
        public string PublicKeyPem { get; set; } = string.Empty;
        public string PrivateKeyPem { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
