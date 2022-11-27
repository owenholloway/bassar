namespace Bassza.Dtos.Financial;

public class ExpeditionsPaymentSummary
{
    public int NoPaymentCount { get; set; } = 0;
    public int Payment1Count { get; set; } = 0;
    public int Payment2Count { get; set; } = 0;
    public int Payment3Count { get; set; } = 0;
    public double TotalPaid { get; set; } = 0.0;
    public double TotalOwed { get; set; } = 0.0;
}