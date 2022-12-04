using Bassza.Api.Dtos;
using Bassza.Api.Dtos.Participant;
using Bassza.Dtos.Financial;
using Bassza.Features.GoogleSheets;

namespace Bassza.Features.Reporting;

public static class OffsiteExtensions
{
    public static async Task UpdateOffsiteFullDaySheet(this SheetsApiManager apiManager, OlemsDataModel dataModel)
    {
        
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
        await apiManager.UpdateRow("c", "OffsiteFullDay", offsiteList, "Activity");
        var dayList = model.Select(pt => pt.Activity.Day.DayOfWeek.ToString()).Cast<object>().ToList();
        await apiManager.UpdateRow("D", "OffsiteFullDay", dayList, "Day");
        
    }
}