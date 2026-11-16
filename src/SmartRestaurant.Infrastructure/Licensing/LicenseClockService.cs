namespace SmartRestaurant.Infrastructure.Licensing;

using SmartRestaurant.Application.Interfaces;

public class LicenseClockService : ILicenseClockService
{
    public DateTime UtcNow => DateTime.UtcNow;

    public bool ValidateClockSanity(DateTime currentTimeUtc, DateTime lastKnownTrustedUtc, TimeSpan tolerance)
    {
        // Nếu đồng hồ hiện tại lùi về quá khứ xa hơn mốc tin cậy gần nhất (vượt quá dung sai tolerance)
        // -> Phát hiện hành vi can thiệp lùi giờ hệ thống (Clock Rollback)
        if (currentTimeUtc < (lastKnownTrustedUtc - tolerance))
        {
            return false;
        }

        return true;
    }
}
