using CommandLine;

namespace Bassza.Features;

public class Options
{
    [Option('u', "username", Required = true, HelpText = "OLEMs Username")]
    public string Username { get; set; } = "";
 
    [Option('p', "password", Required = true, HelpText = "OLEMs Password")]
    public string Password { get; set; } = "";
    
    
    [Option('s', "google-sheet-id", Required = false, HelpText = "Google Sheet Id")]
    public string? GoogleSheetId { get; set; }
    
    [Option('k', "totp-key", Required = false, HelpText = "TOTP Key")]
    public string? TotpKey { get; set; }
    
}