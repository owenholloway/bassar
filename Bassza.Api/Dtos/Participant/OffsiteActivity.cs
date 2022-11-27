namespace Bassza.Api.Dtos.Participant;

public class OffsiteActivity
{
    public string Name { get; set; } = "";
    public DateOnly Day { get; set; }
    public ActivitySession Session { get; set; }
    public double Cost { get; set; }
}