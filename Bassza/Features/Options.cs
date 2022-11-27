using CommandLine;

namespace Bassza.Features;

public class Options
{
    [Option('u', "username", Required = true, HelpText = "OLEMs Username")]
    public string Username { get; set; } = "";
 
    [Option('p', "password", Required = true, HelpText = "OLEMs Password")]
    public string Password { get; set; } = "";
    
}