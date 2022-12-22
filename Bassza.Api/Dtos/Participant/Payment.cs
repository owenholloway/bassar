namespace Bassza.Api.Dtos.Participant;

public class Payment
{
    public int MootId { get; set; }
    
    public string PaymentName { get; set; }

    public DateOnly DueDate { get; set; }
    public double DueValue { get; set; }
    
    public DateOnly? ReceivedDate { get; set; }
    public double? ReceivedValue { get; set; }

    public bool IsOutstanding => ReceivedDate == null || (DueValue > ReceivedValue && DueValue != 0);
    
    public string PaymentIdOrComment { get; set; }
}