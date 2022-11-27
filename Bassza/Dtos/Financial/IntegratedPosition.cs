namespace Bassza.Dtos.Financial;

public class IntegratedPosition
{
    // Staff Participants Base Fee
    public BasePaymentSummary StaffBasePayment 
        = new BasePaymentSummary();
    public ExpeditionsPaymentSummary StaffExpeditionPayment 
        = new ExpeditionsPaymentSummary();
    
    // Full Fee Participants Base Fee
    public BasePaymentSummary FullFeeBasePayment 
        = new BasePaymentSummary();
    public ExpeditionsPaymentSummary FullFeeExpeditionPayment 
        = new ExpeditionsPaymentSummary();
    
    public double TotalBaseOwing
        => Math.Round(StaffBasePayment.TotalOwed
                      + FullFeeBasePayment.TotalOwed, 2);
    public double TotalBasePaid        
        => Math.Round(StaffBasePayment.TotalPaid
                      + FullFeeBasePayment.TotalPaid, 2);
    public double TotalBaseValue 
        => Math.Round(TotalBasePaid 
                      + -TotalBaseOwing, 2);
    
    public double TotalExpeditionOwing
        => -Math.Round(StaffExpeditionPayment.TotalOwed
                      + FullFeeExpeditionPayment.TotalOwed, 2);

    public double TotalExpeditionPaid
        => Math.Round(StaffExpeditionPayment.TotalPaid
                      + FullFeeExpeditionPayment.TotalPaid, 2);
    public double TotalExpeditionValue 
        => Math.Round(TotalExpeditionPaid 
                      + -TotalExpeditionOwing, 2);
    
    public int TotalPayingParticipants 
        => StaffBasePayment.Participants 
           + FullFeeBasePayment.Participants;


    public double TotalCalculatedPaid
        => Math.Round(TotalBasePaid
                      + TotalExpeditionPaid, 2);

    public double TotalCalculatedValue
        => Math.Round(TotalBaseValue
                      + TotalExpeditionValue, 2);
    
    private bool _resolved = false;
    public virtual bool Resolved
    {
        get => _resolved;
        set { if (!_resolved)_resolved = value; }
    }
}