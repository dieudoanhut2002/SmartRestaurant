namespace SmartRestaurant.Application.Services;

using System.Text;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class VietQrService : IVietQrService
{
    // Default Napas Beneficiary Bank (e.g. MBBank: 970422 / VietinBank: 970415)
    private const string DefaultBankBin = "970422"; // MBBank
    private const string DefaultBankName = "MBBank (Ngân hàng Quân Đội)";
    private const string DefaultAccountNumber = "0988888888";
    private const string DefaultAccountName = "NHA HANG SMART RESTAURANT";

    public VietQrPayloadDto GenerateDynamicVietQr(
        string invoiceNumber,
        decimal amount,
        string? bankBin = null,
        string? accountNumber = null,
        string? accountName = null)
    {
        string bin = string.IsNullOrWhiteSpace(bankBin) ? DefaultBankBin : bankBin.Trim();
        string accNo = string.IsNullOrWhiteSpace(accountNumber) ? DefaultAccountNumber : accountNumber.Trim();
        string accName = string.IsNullOrWhiteSpace(accountName) ? DefaultAccountName : accountName.Trim().ToUpperInvariant();
        long intAmount = (long)Math.Round(amount, 0, MidpointRounding.AwayFromZero);
        string sanitizedInvoice = invoiceNumber.Replace("-", "").Trim().ToUpperInvariant();

        // Build EMVCo Payload
        // 00: Payload Format Indicator (01)
        string f00 = BuildTlv("00", "01");
        // 01: Point of Initiation Method (12 = Dynamic QR)
        string f01 = BuildTlv("01", "12");

        // 38: Napas Merchant Account Information
        string napasAid = BuildTlv("00", "A000000727");
        string napasBin = BuildTlv("01", bin);
        string napasAcc = BuildTlv("02", accNo);
        string f38Value = napasAid + napasBin + napasAcc;
        string f38 = BuildTlv("38", f38Value);

        // 53: Transaction Currency (704 = VND)
        string f53 = BuildTlv("53", "704");
        // 54: Transaction Amount
        string f54 = BuildTlv("54", intAmount.ToString());
        // 58: Country Code (VN)
        string f58 = BuildTlv("58", "VN");

        // 62: Additional Data Field (Reference / Order Code)
        string refInfo = BuildTlv("08", sanitizedInvoice);
        string f62 = BuildTlv("62", refInfo);

        // Concatenate without CRC
        string payloadWithoutCrc = f00 + f01 + f38 + f53 + f54 + f58 + f62 + "6304";

        // Calculate CRC16-CCITT
        ushort crc = ComputeCrc16(Encoding.ASCII.GetBytes(payloadWithoutCrc));
        string crcHex = crc.ToString("X4");

        string fullEmvCo = payloadWithoutCrc + crcHex;

        // Standard VietQR Quicklink
        string quickLink = $"https://img.vietqr.io/image/{bin}-{accNo}-compact2.png?amount={intAmount}&addInfo={Uri.EscapeDataString(sanitizedInvoice)}&accountName={Uri.EscapeDataString(accName)}";

        return new VietQrPayloadDto(
            QrString: fullEmvCo,
            BankBin: bin,
            BankName: DefaultBankName,
            AccountNumber: accNo,
            AccountName: accName,
            Amount: intAmount,
            OrderCode: sanitizedInvoice,
            QuickLink: quickLink
        );
    }

    public bool ValidateCrc16(string emvCoPayload)
    {
        if (string.IsNullOrWhiteSpace(emvCoPayload) || emvCoPayload.Length < 8)
            return false;

        string payloadWithoutCrc = emvCoPayload[..^4];
        string expectedCrcHex = emvCoPayload[^4..].ToUpperInvariant();

        ushort computed = ComputeCrc16(Encoding.ASCII.GetBytes(payloadWithoutCrc));
        string computedHex = computed.ToString("X4");

        return string.Equals(expectedCrcHex, computedHex, StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildTlv(string tag, string value)
    {
        return $"{tag}{value.Length:D2}{value}";
    }

    private static ushort ComputeCrc16(byte[] data)
    {
        ushort crc = 0xFFFF;
        foreach (byte b in data)
        {
            crc ^= (ushort)(b << 8);
            for (int i = 0; i < 8; i++)
            {
                if ((crc & 0x8000) != 0)
                    crc = (ushort)((crc << 1) ^ 0x1021);
                else
                    crc = (ushort)(crc << 1);
            }
        }
        return crc;
    }
}
