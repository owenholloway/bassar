using Bassza.Api.Dtos;
using Bassza.Api.Dtos.Participant;
using Bassza.Dtos;
using Bassza.Dtos.Financial;

namespace Bassza.Features;

public static class IntegratedPositionExtensions
{

    public static IntegratedPosition CalculatePosition(this OlemsDataModel dataModel)
    {
        var obj = new IntegratedPosition();
        
        var staffPayees = dataModel
            .Participants
            .Where(pt => Math.Abs(pt.FinancialPosition.BaseFee - 600) < 0.1
                         && pt.FinancialPosition.Expedition > -1);
        var staffParticipants = staffPayees as Participant[] ?? staffPayees.ToArray();

        obj.StaffBasePayment = staffParticipants.ResolveBasePayment();
        obj.StaffExpeditionPayment = staffParticipants.ResolveExpeditions();
        
        var fullFeePayees = dataModel
            .Participants
            .Where(pt => pt.PayingParticipant
                         && !(Math.Abs(pt.FinancialPosition.BaseFee - 600.00) < 0.1));

        
        var fullFeeParticipants = fullFeePayees as Participant[] ?? fullFeePayees.ToArray();
        
        obj.FullFeeBasePayment = fullFeeParticipants.ResolveBasePayment();
        obj.FullFeeExpeditionPayment = fullFeeParticipants.ResolveExpeditions();
        
        return obj;
    }
    
    private static ExpeditionsPaymentSummary ResolveExpeditions(
        this IReadOnlyCollection<Participant> participants)
    {
        return new ExpeditionsPaymentSummary()
        {
            NoPaymentCount = participants
                .Count(pt => !pt.FinancialPosition.Expedition1Complete 
                             && !pt.FinancialPosition.Expedition2Complete),
            
            Payment1Count = participants
                .Count(pt => (pt.FinancialPosition.Expedition1Complete && !pt.FinancialPosition.Expedition2Complete)),
            
            Payment2Count = participants
                .Count(pt => pt.FinancialPosition.Expedition2Complete),

            Payment3Count = 0,
            
            TotalPaid = participants.Sum(pt => pt.FinancialPosition.ExpeditionFeeSum),
            TotalOwed = participants.Sum(pt => pt.FinancialPosition.ExpeditionFeeOwed)
        };
        
    }
    
    private static BasePaymentSummary ResolveBasePayment(
        this IReadOnlyCollection<Participant> participants)
    {
        return new BasePaymentSummary
        {
            Participants = participants.Count,
            NoPaymentCount = participants
                .Count(pt => pt.FinancialPosition.NoBaseFeePayment),
            Payment1Count = participants
                .Count(pt => pt.FinancialPosition.Payment1Complete && !(pt.FinancialPosition.Payment2Complete || pt.FinancialPosition.Payment3Complete) ),
            Payment2Count = participants
                .Count(pt => pt.FinancialPosition.Payment2Complete&& !(pt.FinancialPosition.Payment3Complete)),
            Payment3Count = participants
                .Count(pt => pt.FinancialPosition.Payment3Complete),
            TotalPaid = participants.Sum(pt => pt.FinancialPosition.BaseFeeSum),
            TotalOwed = participants.Sum(pt => pt.FinancialPosition.BaseFeeOwed)
        };

    }
    
}