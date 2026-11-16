namespace SmartRestaurant.Domain.Exceptions;

public class LicenseValidationException(string message) : DomainException(message)
{
}

public class LicenseTamperedException(string message = "Chữ ký số bản quyền không hợp lệ hoặc tệp giấy phép đã bị chỉnh sửa trái phép.")
    : DomainException(message)
{
}

public class LicenseLimitExceededException(string message) : DomainException(message)
{
}
