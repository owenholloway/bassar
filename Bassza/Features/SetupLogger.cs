using Serilog;
using Serilog.Core;

namespace Bassza.Features;

public class SetupLogger
{
    private readonly ILogger _logger;
    
    public SetupLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void Run()
    {
        Log.Logger = _logger;
    }
    
}