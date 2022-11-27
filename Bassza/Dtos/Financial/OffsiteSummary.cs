namespace Bassza.Dtos.Financial;

public class OffsiteSummary
{
    public int NoPaymentCount { get; set; } = 0;
    public int HalfPaymentCount { get; set; } = 0;
    public int FullPaymentCount { get; set; } = 0;
    public double TotalPaid { get; set; } = 0.0;
    public double TotalOwed { get; set; } = 0.0;
}