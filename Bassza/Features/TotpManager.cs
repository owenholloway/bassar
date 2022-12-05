using OtpNet;
using Serilog;
namespace Bassza.Features;
public class TotpManager
{
    private readonly Totp _totp;
    public TotpManager(Options options)
    {
        var key = options!.TotpKey;
        var keyBytes = Base32Encoding.ToBytes(key);
        
        _totp = new Totp(keyBytes, mode: OtpHashMode.Sha1, step:30);
        
    }
    public string GetTotp()
    {
        return _totp.ComputeTotp(DateTime.UtcNow);
    }
    
}