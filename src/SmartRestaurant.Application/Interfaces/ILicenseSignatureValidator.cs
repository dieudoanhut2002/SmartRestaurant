namespace SmartRestaurant.Application.Interfaces;

/// <summary>
/// Giao diện kiểm thực chữ ký số bất đối xứng (ECDSA P-256) của tài liệu bản quyền
/// Tuân thủ quy định tại Chỉ thị Chủ Dự Án (Owner Directive §3.3)
/// </summary>
public interface ILicenseSignatureValidator
{
    /// <summary>
    /// Kiểm thực chữ ký số trên chuỗi payload đã được tuần tự hóa xác định (Deterministic Serialization)
    /// </summary>
    bool VerifySignature(string serializedPayload, string signatureBase64);
}
