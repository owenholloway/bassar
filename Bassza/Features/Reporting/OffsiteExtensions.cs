using Bassza.Api.Dtos;
using Bassza.Api.Dtos.Participant;
using Bassza.Dtos.Financial;
using Bassza.Features.GoogleSheets;
using Serilog;

namespace Bassza.Features.Reporting;

public static class OffsiteExtensions
{
    public static async Task UpdateOffsiteFullDaySheet(this SheetsApiManager apiManager, OlemsDataModel dataModel)
    {
        await Signals.Requestors.WaitAsync();
        Log.Information("UpdateOffsiteFullDaySheet Start");
        var report = dataModel
            .Participants
            .Where(pt => pt.OffsiteActivities
            .Any(at => at.Session == ActivitySession.FullDay));

        var model = new List<OffsiteInfo>();
        
        foreach (var participant in report)
        {
            foreach (var participantOffsiteActivity in participant.OffsiteActivities)
            {
                model.Add(new OffsiteInfo()
                {
                    ParticipantId = participant.EventId,
                    ParticipantName = participant.Name,
                    Activity = participantOffsiteActivity
                });
            }
        }
        
        if (!apiManager.IsActive) return;

        report = report.OrderBy(pt => pt.EventId).ToList();
        
        var idList = model.Select(pt => pt.ParticipantId).Cast<object>().ToList();
        await apiManager.UpdateRow("A", "OffsiteFullDay", idList, "Id");
        var nameList = model.Select(pt => pt.ParticipantName).Cast<object>().ToList();
        await apiManager.UpdateRow("B", "OffsiteFullDay", nameList, "Name");
        var offsiteList = model.Select(pt => pt.Activity.Name).Cast<object>().ToList();
        await apiManager.UpdateRow("B", "OffsiteFullDay", offsiteList, "Activity");
        var dayList = model.Select(pt => pt.Activity.Day.DayOfWeek.ToString()).Cast<object>().ToList();
        await apiManager.UpdateRow("D", "OffsiteFullDay", dayList, "Day");
        
        
        Log.Information("UpdateOffsiteFullDaySheet End");
        Signals.ResetRequestor();
        
    }
    
    public static async Task UpdateOffsiteEmails(this SheetsApiManager apiManager, OlemsDataModel dataModel)
    {
        if (!apiManager.IsActive) return;
        
        var report = dataModel
            .Participants
            .Where(pt => pt.OffsiteActivities.Any());

        var model = new List<OffsiteInfo>();
        
        foreach (var participant in report)
        {
            foreach (var participantOffsiteActivity in participant.OffsiteActivities)
            {
                model.Add(new OffsiteInfo()
                {
                    ParticipantId = participant.EventId,
                    ParticipantName = participant.Name,
                    ParticipantEmail = participant.EmailPrimary,
                    Activity = participantOffsiteActivity,
                    NameDaySession = $"{participantOffsiteActivity.Name} " +
                                     $"| {participantOffsiteActivity.Day.DayOfWeek} " +
                                     $"| {participantOffsiteActivity.Session}"
                });
            }
        }

        var grouped = model
            .GroupBy(oi => oi.NameDaySession)
            .OrderBy(oi => oi.Key);

        var colA = new List<object>();
        var colB = new List<object>();
        
        foreach (var offsiteInfos in grouped)
        {
            colA.Add(offsiteInfos.Key);
            colB.Add("");
            foreach (var offsiteInfo in offsiteInfos)
            {
                colA.Add(offsiteInfo.ParticipantName);
                colB.Add(offsiteInfo.ParticipantEmail);
            }
            colA.Add("");
            colB.Add("");
        }
        
        
        await apiManager.UpdateRow("A", "OffsiteEmails", colA, "Name");
        
        await apiManager.UpdateRow("B", "OffsiteEmails", colB, "Email");
        
        await Signals.Requestors.WaitAsync();
        Log.Information("UpdateOffsiteEmails Start");
        
        Log.Information("UpdateOffsiteEmails End");
        Signals.ResetRequestor();
    }
}