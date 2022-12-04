using Bassza.Api.Dtos;
using Bassza.Api.Dtos.Participant;
using Bassza.Dtos.Financial;
using Bassza.Features.GoogleSheets;

namespace Bassza.Features.Reporting;

public static class OffsiteDietaryExtensions
{
    public static List<OffsiteInfo> ProcessOffsiteDietaries(this OlemsDataModel dataModel)
    {

        var records = new List<OffsiteInfo>();

        foreach (var dataModelParticipant in dataModel.Participants)
        {
            var dietaries
                = dataModelParticipant
                    .MedicalInformation
                    .Where(dt => dt.MedicalInformationType == MedicalInformationType.DietaryRequirements
                                 || (dt.MedicalInformationType == MedicalInformationType.Allergies && dt.Name.Contains("Foods:")));
            if (!dietaries.Any()) continue;

            foreach (var offsiteActivity in dataModelParticipant.OffsiteActivities)
            {
                foreach (var medicalInformation in dietaries)
                {
                    
                    records.Add(new OffsiteInfo()
                    {
                        ParticipantId = dataModelParticipant.EventId,
                        ParticipantName = dataModelParticipant.Name,
                        Activity = offsiteActivity,
                        MedicalInformation = medicalInformation
                    });
                    
                }
            }

        }

        return records;


    }


    public static async Task UpdateOffsiteDietariesSheet(this SheetsApiManager apiManager, List<OffsiteInfo> report)
    {
        
        if (!apiManager.IsActive) return;

        report = report.OrderBy(pt => pt.ParticipantId).ToList();
        
        var idList = report.Select(pt => pt.ParticipantId).Cast<object>().ToList();
        await apiManager.UpdateRow("A", "OffsiteDietary", idList, "Id");
        var nameList = report.Select(pt => pt.ParticipantName).Cast<object>().ToList();
        await apiManager.UpdateRow("B", "OffsiteDietary", nameList, "Name");
        var offsiteList = report.Select(pt => pt.Activity.Name).Cast<object>().ToList();
        await apiManager.UpdateRow("c", "OffsiteDietary", offsiteList, "Activity");
        var dayList = report.Select(pt => pt.Activity.Day.DayOfWeek.ToString()).Cast<object>().ToList();
        await apiManager.UpdateRow("D", "OffsiteDietary", dayList, "Day");
        var allergyList = report.Select(pt => pt.MedicalInformation.Name).Cast<object>().ToList();
        await apiManager.UpdateRow("E", "OffsiteDietary", allergyList, "Dietary");
        
    }
    
}   