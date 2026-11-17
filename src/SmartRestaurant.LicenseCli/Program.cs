namespace SmartRestaurant.LicenseCli;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Infrastructure.Licensing;

public class Program
{
    private const string IssuerPrivateKeyPem = """
    -----BEGIN PRIVATE KEY-----
    MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgcu2DWSWi7imzTNRI
    gSKuLJ0X6iYHuVVB5ExlA0b0Q6ihRANCAAQ2BRp5LfpOfLnA5qKxqQK+gD9/0P26
    e31W5jbIcRdqXHPUxlh6tly9MZhO8AcwIFgk9XK3aNRGDLrRD0YJibGN
    -----END PRIVATE KEY-----
    """;

    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("================================================================================");
        Console.WriteLine(" SMARTRESTAURANT — CÔNG CỤ QUẢN TRỊ BẢN QUYỀN OFFLINE (LICENSE CLI)");
        Console.WriteLine("================================================================================\n");

        string customerId = "CUST-PILOT-STORE-01";
        string installationId = "*";
        string edition = "Enterprise";
        int validityDays = 365;
        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "pilot-store-license.lic");

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--customer" && i + 1 < args.Length) customerId = args[++i];
            else if (args[i] == "--installation" && i + 1 < args.Length) installationId = args[++i];
            else if (args[i] == "--edition" && i + 1 < args.Length) edition = args[++i];
            else if (args[i] == "--days" && i + 1 < args.Length && int.TryParse(args[++i], out var d)) validityDays = d;
            else if (args[i] == "--out" && i + 1 < args.Length) outputPath = args[++i];
            else if (args[i] == "--inspect" && i + 1 < args.Length)
            {
                return InspectLicense(args[++i]);
            }
        }

        var now = DateTime.UtcNow;
        var validFrom = now.AddHours(-1);
        var expiresAt = now.AddDays(validityDays);
        var licenseId = $"LIC-{now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

        var model = new LicenseModel
        {
            LicenseId = licenseId,
            CustomerId = customerId,
            InstallationId = installationId,
            ProductCode = "SmartRestaurant",
            Edition = edition,
            Features =
            [
                "FloorManagement",
                "MenuManagement",
                "WaiterPOS",
                "KitchenDisplay",
                "CashierBilling",
                "VietQR247",
                "SplitBilling",
                "InventoryBOM",
                "FinancialReporting",
                "OfflineLicensing"
            ],
            IssuedAt = now,
            ValidFrom = validFrom,
            ExpiresAt = expiresAt,
            LicenseVersion = 1,
            GracePolicy = new GracePolicyDto
            {
                OfflineAllowanceDays = 60,
                ExpiryGraceDays = 14,
                ExpiringSoonDays = 30
            }
        };

        var payloadJson = JsonSerializer.Serialize(model, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(IssuerPrivateKeyPem);

        var data = Encoding.UTF8.GetBytes(payloadJson.Trim());
        var signatureBytes = ecdsa.SignData(data, HashAlgorithmName.SHA256);
        var signatureBase64 = Convert.ToBase64String(signatureBytes);

        var envelope = LicenseStorageService.PackageLicenseEnvelope(payloadJson, signatureBase64);

        var fullPath = Path.GetFullPath(outputPath);
        var dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(fullPath, envelope, Encoding.UTF8);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("-> Tạo giấy phép bản quyền có chữ ký số ECDSA P-256 thành công!");
        Console.ResetColor();
        Console.WriteLine($"  - Mã giấy phép (LicenseId):     {licenseId}");
        Console.WriteLine($"  - Mã khách hàng (CustomerId):   {customerId}");
        Console.WriteLine($"  - Máy chủ định danh:            {installationId} (Áp dụng toàn bộ máy chủ Pilot)");
        Console.WriteLine($"  - Phiên bản (Edition):          {edition}");
        Console.WriteLine($"  - Thời hạn:                     {validFrom:dd/MM/yyyy} -> {expiresAt:dd/MM/yyyy} ({validityDays} ngày)");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  - Tệp lưu trữ:                  {fullPath}");
        Console.ResetColor();
        Console.WriteLine("\nGiấy phép đã sẵn sàng kích hoạt tại Quản trị hệ thống: /License/Index\n");

        return 0;
    }

    private static int InspectLicense(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Lỗi: Không tìm thấy tệp {filePath}");
            Console.ResetColor();
            return 1;
        }

        var content = File.ReadAllText(filePath, Encoding.UTF8);
        var doc = LicenseStorageService.ParseLicenseEnvelope(content);
        if (doc == null || doc.License == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Lỗi: Tệp giấy phép không hợp lệ hoặc cấu trúc envelope bị hỏng.");
            Console.ResetColor();
            return 1;
        }

        var lic = doc.License;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("THÔNG TIN CHI TIẾT GIẤY PHÉP:");
        Console.ResetColor();
        Console.WriteLine($"  - LicenseId:      {lic.LicenseId}");
        Console.WriteLine($"  - CustomerId:     {lic.CustomerId}");
        Console.WriteLine($"  - InstallationId: {lic.InstallationId}");
        Console.WriteLine($"  - ProductCode:    {lic.ProductCode}");
        Console.WriteLine($"  - Edition:        {lic.Edition}");
        Console.WriteLine($"  - IssuedAt:       {lic.IssuedAt:dd/MM/yyyy HH:mm:ss} UTC");
        Console.WriteLine($"  - ValidFrom:      {lic.ValidFrom:dd/MM/yyyy HH:mm:ss} UTC");
        Console.WriteLine($"  - ExpiresAt:      {lic.ExpiresAt:dd/MM/yyyy HH:mm:ss} UTC");
        Console.WriteLine($"  - Features:       {string.Join(", ", lic.Features)}");

        return 0;
    }
}
