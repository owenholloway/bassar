namespace Bassza.Features;

public class Signals
{
    public AutoResetEvent ApplicationDone { get; set; }

    public Signals()
    {
        ApplicationDone = new AutoResetEvent(false);
    }
        
}