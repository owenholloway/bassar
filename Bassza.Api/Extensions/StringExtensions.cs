using Serilog;

namespace Bassza.Api.Extensions;

public static class StringExtensions
{
    public static string TrimFormatting(this string value)
    {
        return value.Replace("$","").Replace("AUD","").Replace("<br>","").Trim();
    }

    public static double TryParseToDouble(this string value)
    {
        var sanitisedValue 
            = value
                .Replace("(", "-")
                .Replace(")", "");
        
        try
        {
            return Double.Parse(sanitisedValue);
        }
        catch (Exception e)
        {
            Log.Warning($"Error parsing value {sanitisedValue}, \"{e.Message}\"");
            return 0.0;
        }
    }
    
}