namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public class GracePolicyDto
{
    public int OfflineAllowanceDays { get; set; } = 30;
    public int ExpiryGraceDays { get; set; } = 7;
    public int ExpiringSoonDays { get; set; } = 14;
}

public class LicenseModel
{
    public string LicenseId { get; set; } = Guid.NewGuid().ToString();
    public string CustomerId { get; set; } = string.Empty;
    public string InstallationId { get; set; } = string.Empty;
    public string ProductCode { get; set; } = "SmartRestaurant";
    public string Edition { get; set; } = "Standard";
    public List<string> Features { get; set; } = [];
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddYears(1);
    public int LicenseVersion { get; set; } = 1;
    public GracePolicyDto GracePolicy { get; set; } = new();
}

public class StoredLicenseDocument
{
    public string RawDocument { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public string SignatureBase64 { get; set; } = string.Empty;
    public LicenseModel License { get; set; } = new();
    public DateTime InstalledAt { get; set; } = DateTime.UtcNow;
}

public class LicenseStateMetadata
{
    public DateTime? LastValidatedAtUtc { get; set; }
    public DateTime LastKnownTrustedUtc { get; set; } = DateTime.UtcNow;
    public long ValidationCount { get; set; }
    public string? ActiveLicenseId { get; set; }
}

public class InstallationIdentityDto
{
    public string InstallationId { get; set; } = string.Empty;
    public string HardwareFingerprint { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public int ProcessorCount { get; set; }
    public string PublicKeyPem { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public class LicenseValidationResult
{
    public bool IsValid { get; set; }
    public LicenseState State { get; set; } = LicenseState.Uninitialized;
    public string StateName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? DaysRemaining { get; set; }
    public int? GraceDaysRemaining { get; set; }
    public int? OfflineDaysRemaining { get; set; }
    public bool IsClockRollbackDetected { get; set; }
    public bool IsOfflineAllowanceExceeded { get; set; }
    public bool CanOperate => State is LicenseState.Active or LicenseState.ExpiringSoon or LicenseState.OfflineGrace or LicenseState.ExpiredGrace;
}

public class LicenseSummaryDto
{
    public LicenseState State { get; set; } = LicenseState.Uninitialized;
    public string StateText { get; set; } = "Chưa nạp bản quyền";
    public string StateBadgeClass { get; set; } = "badge bg-secondary";
    public string LicenseId { get; set; } = "N/A";
    public string CustomerId { get; set; } = "N/A";
    public string InstallationId { get; set; } = "N/A";
    public string ProductCode { get; set; } = "SmartRestaurant";
    public string Edition { get; set; } = "Standard";
    public List<string> Features { get; set; } = [];
    public DateTime IssuedAt { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int LicenseVersion { get; set; } = 1;
    public int? DaysRemaining { get; set; }
    public int? GraceDaysRemaining { get; set; }
    public int? OfflineDaysRemaining { get; set; }
    public bool IsDevelopmentMode { get; set; }
    public InstallationIdentityDto? InstallationIdentity { get; set; }
    public bool CanOperate => State is LicenseState.Active or LicenseState.ExpiringSoon or LicenseState.OfflineGrace or LicenseState.ExpiredGrace;
}

public class LicenseActivationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public LicenseState State { get; set; } = LicenseState.Uninitialized;
    public LicenseSummaryDto? Summary { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class LicenseOptions
{
    public const string SectionName = "Licensing";

    public string Mode { get; set; } = "Development";
    public string ProductCode { get; set; } = "SmartRestaurant";
    public string? IssuerPublicKeyPem { get; set; }
    public int DefaultOfflineAllowanceDays { get; set; } = 30;
    public int DefaultExpiryGraceDays { get; set; } = 7;
    public int ExpiringSoonDays { get; set; } = 14;
    public int ClockDriftToleranceMinutes { get; set; } = 10;
    public string StorageDirectory { get; set; } = string.Empty;
}
