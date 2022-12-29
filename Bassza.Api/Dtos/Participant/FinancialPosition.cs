namespace Bassza.Api.Dtos.Participant;

public class FinancialPosition
{
    // Payment Details
    public double BaseFee { get; set; } = 0.00;
    public double Expedition { get; set; } = 0.00;
    public double OffsiteHolds { get; set; } = 0.00;

    public double MerchPurchases { get; set; } = 0.00;

    public double Due => BaseFee + 
                         Expedition + 
                         OffsiteHolds + 
                         MerchPurchases;
    
    public double NoMootFee { get; set; } = 0.00;
    public double Payment1 { get; set; } = 0.00;
    public double Payment2 { get; set; } = 0.00;
    public double Payment3 { get; set; } = 0.00;
    public double BasePaymentTotal => Payment1 + 
                                      Payment2 + 
                                      Payment3;
    
    public double ExpeditionPayment1 { get; set; } = 0.00;
    public double ExpeditionPayment2 { get; set; } = 0.00;
    public double ExpeditionPayment3 { get; set; } = 0.00;
    public double ExpeditionPaymentTotal => ExpeditionPayment1 + 
                                            ExpeditionPayment2 + 
                                            ExpeditionPayment3;
    
    public double OffSitePayment { get; set; } = 0.00;
    public double OffSiteHold { get; set; } = 0.00;
    public double OtherPayment { get; set; } = 0.00;

    public double Refunds { get; set; } = 0.00;

    public bool HaveTentPayment = false;
    public bool TentPaymentComplete = false;

    public double Paid => BasePaymentTotal +
                          ExpeditionPaymentTotal;


    public double Outstanding => Due - Paid;
    
    public List<Payment> Payments = new List<Payment>();

    public double OffsiteFeeOwed => OffsiteHolds - OffSitePayment;
    
    public double ExpeditionFeeCompletedSum => ExpeditionPaymentTotal;
    public double ExpeditionFeeOwed => Expedition - ExpeditionPaymentTotal;

    public bool Expedition1Complete { get; set; } = false;
    public bool Expedition2Complete { get; set; } = false;
    public bool Expedition3Complete { get; set; } = false;
    public bool NoExpeditionFeePayment => !(Expedition1Complete || Expedition2Complete || Expedition3Complete);
    
    public double BaseFeeCompletedSum => BasePaymentTotal;
    public double BaseFeeOwed => BaseFee - BaseFeeCompletedSum;

    public bool Payment1Complete { get; set; } = false;
    public bool Payment2Complete { get; set; } = false;
    public bool Payment3Complete { get; set; } = false;
    public bool NoBaseFeePayment => !(Payment1Complete || Payment2Complete || Payment3Complete);

}