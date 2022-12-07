namespace Bassza.Api.Dtos.Participant;

public class Participant
{
    public int EventId { get; set; } = -1;
    
    public string NameFirst { get; set; } = "";
    public string NameLast { get; set; } = "";

    public string Status { get; set; } = "";

    public string Name => NameLast + ", " + NameFirst;

    public bool PayingParticipant => !Status.ToLower().Contains("not proceeding")
                                     && !Status.ToLower().Contains("withdrawn")
                                     && !Status.ToLower().Contains("initial")
                                     && EventId != 1
                                     && EventId != 3
                                     && !Contingent.ToLower().Contains("aimmot")
                                     && FinancialPosition.BaseFee > 10;

    public bool IsStaff => FinancialPosition.BaseFee is > 599 and < 601;
    
    public DateTime DateOfBirth { get; set; } = DateTime.MinValue;

    public TimeSpan Age => DateTime.Now - DateOfBirth;
        
    // Contact Details
    public string Contingent { get; set; } = "";
    public int MemberShipNo { get; set; } = -1;
    public string EmailPrimary { get; set; } = "";
    public string EmailSecondary { get; set; } = "";

    // Activities & Expeditions
    public string Expedition { get; set; } = "No Expedition";
    public string ExpeditionUnit { get; set; } = "N/A";
    public readonly List<OffsiteActivity> OffsiteActivities = new();

    private double TotalOffsiteCost => OffsiteActivities.Sum(at => at.Cost);
    public bool OffsiteDiscrepancy => Math.Abs(TotalOffsiteCost - FinancialPosition.OffsiteHolds) > 0.1;

    public bool OffsitePaidFor => TotalOffsiteCost <= FinancialPosition.OffSitePayment;
    
    public FinancialPosition FinancialPosition { get; set; } = new FinancialPosition();
    
    public IEnumerable<MedicalInformation> MedicalInformation = new List<MedicalInformation>();

}