namespace Bassza.Features;

public class Signals
{
    public AutoResetEvent ApplicationDone { get; set; }

    public static SemaphoreSlim Requestors { get; set; } = new SemaphoreSlim(1, 1);

    public Signals()
    {
        ApplicationDone = new AutoResetEvent(false);
    }

    public static async Task ResetRequestor()
    {
        await Task.Delay(30000);
        Requestors.Release(1);
    }
    
}