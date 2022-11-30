namespace Bassza.Api.Dtos.Participant;

public class FinancialPosition
{
    // Payment Details
    public double BaseFee { get; set; } = 0.00;
    public double Expedition { get; set; } = 0.00;
    public double OffsiteHolds { get; set; } = 0.00;
    public double Due { get; set; } = 0.00;
    public double Paid { get; set; } = 0.00;
    public double ExpeditionPayment1 { get; set; } = 0.00;
    public double ExpeditionPayment2 { get; set; } = 0.00;
    public double ExpeditionPayment3Tba { get; set; } = 0.00;
    public double NoMootFee { get; set; } = 0.00;
    public double Payment1 { get; set; } = 0.00;
    public double Payment2 { get; set; } = 0.00;
    public double Payment3 { get; set; } = 0.00;
    public double OffSitePayment { get; set; } = 0.00;
    public double OffSiteHold { get; set; } = 0.00;
    public double OtherPayment { get; set; } = 0.00;
    public double Refunds { get; set; } = 0.00;
    public double Outstanding { get; set; } = 0.00;

    public double ExpeditionFeeSum => ExpeditionPayment1 + ExpeditionPayment2 + ExpeditionPayment3Tba;
    public double ExpeditionFeeOwed =>  Expedition - ExpeditionFeeSum < 0 ? 0 : Expedition - ExpeditionFeeSum;

    public bool Expedition1Complete => Expedition > 0 && ExpeditionFeeSum >= Expedition/2;
    public bool Expedition2Complete => Expedition > 0 && ExpeditionFeeSum >= Expedition;

    public bool NoExpeditionFeePayment => !(Expedition1Complete || Expedition2Complete);
    
    public double BaseFeeSum => Payment1 + Payment2 + Payment3;
    public double BaseFeeOwed => -(BaseFeeSum - BaseFee) < 0 ? 0 : -(BaseFeeSum - BaseFee);
    
    public bool Payment1Complete => Payment1 > 199;
    public bool Payment2Complete => Payment1Complete && Payment2 > 199;
    public bool Payment3Complete => Payment2Complete && Payment3 > 199;
    public bool NoBaseFeePayment => !(Payment1Complete || Payment2Complete || Payment3Complete);

}