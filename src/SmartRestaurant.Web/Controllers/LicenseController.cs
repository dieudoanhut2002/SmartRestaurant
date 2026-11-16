namespace SmartRestaurant.Web.Controllers;

using System.Text;
using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class LicenseController(
    ILicenseService licenseService,
    IInstallationIdentityService installationIdentityService,
    ILogger<LicenseController> logger) : Controller
{
    private readonly ILicenseService _licenseService = licenseService;
    private readonly IInstallationIdentityService _installationIdentityService = installationIdentityService;
    private readonly ILogger<LicenseController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Bản Quyền Hệ Thống";
        var summary = await _licenseService.GetSummaryAsync();
        var identity = await _installationIdentityService.GetOrCreateInstallationIdentityAsync();
        ViewBag.InstallationIdentity = identity;

        return View(summary);
    }

    [HttpGet]
    public async Task<IActionResult> InstallationIdentity()
    {
        var identity = await _installationIdentityService.GetOrCreateInstallationIdentityAsync();
        return Json(new { success = true, data = identity });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(IFormFile? licenseFile, string? licenseText)
    {
        string? contentToActivate = null;

        if (licenseFile != null && licenseFile.Length > 0)
        {
            using var reader = new StreamReader(licenseFile.OpenReadStream(), Encoding.UTF8);
            contentToActivate = await reader.ReadToEndAsync();
        }
        else if (!string.IsNullOrWhiteSpace(licenseText))
        {
            contentToActivate = licenseText.Trim();
        }

        if (string.IsNullOrWhiteSpace(contentToActivate))
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new LicenseActivationResultDto
                {
                    Success = false,
                    Message = "Vui lòng chọn tệp giấy phép .lic hoặc dán chuỗi bản quyền hợp lệ.",
                    Errors = ["Không tìm thấy nội dung bản quyền."]
                });
            }

            TempData["ErrorMessage"] = "Vui lòng chọn tệp giấy phép .lic hoặc dán chuỗi bản quyền.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _licenseService.InstallLicenseAsync(contentToActivate);

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return Json(result);
        }

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            var errorDetails = result.Errors.Count > 0 ? " — " + string.Join(" | ", result.Errors) : string.Empty;
            TempData["ErrorMessage"] = $"{result.Message}{errorDetails}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ExportInstallationInfo()
    {
        var identity = await _installationIdentityService.GetOrCreateInstallationIdentityAsync();

        var sb = new StringBuilder();
        sb.AppendLine("======================================================================");
        sb.AppendLine("SMARTRESTAURANT — HỒ SƠ ĐỊNH DANH MÁY CHỦ CÀI ĐẶT (INSTALLATION IDENTITY)");
        sb.AppendLine("======================================================================");
        sb.AppendLine($"Mã Cài Đặt (InstallationId):        {identity.InstallationId}");
        sb.AppendLine($"Mã Phần Cứng (Hardware Fingerprint): {identity.HardwareFingerprint}");
        sb.AppendLine($"Tên Máy Chủ (Machine Name):         {identity.MachineName}");
        sb.AppendLine($"Hệ Điều Hành (Operating System):    {identity.OperatingSystem}");
        sb.AppendLine($"Số Lõi Vi Xử Lý (Processor Cores):  {identity.ProcessorCount}");
        sb.AppendLine($"Thời Điểm Khởi Tạo (UTC):           {identity.CreatedAtUtc:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("======================================================================");
        sb.AppendLine("HƯỚNG DẪN:");
        sb.AppendLine("Gửi thông tin này cho Đội ngũ Cấp phép SmartRestaurant để nhận");
        sb.AppendLine("tệp giấy phép bản quyền (.lic) được ký số riêng cho máy chủ này.");
        sb.AppendLine("======================================================================");

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var fileName = $"SmartRestaurant-InstallationInfo-{DateTime.UtcNow:yyyyMMddHHmmss}.txt";

        return File(bytes, "text/plain; charset=utf-8", fileName);
    }

    [HttpGet]
    public async Task<IActionResult> ExportRehostRequest()
    {
        var rehostToken = await _installationIdentityService.ExportRehostRequestAsync();
        var bytes = Encoding.UTF8.GetBytes(rehostToken);
        var fileName = $"SmartRestaurant-RehostRequest-{DateTime.UtcNow:yyyyMMddHHmmss}.req";

        return File(bytes, "application/octet-stream", fileName);
    }
}
