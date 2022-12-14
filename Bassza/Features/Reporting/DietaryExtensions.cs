using Bassza.Api.Dtos;
using Bassza.Api.Dtos.Participant;
using Bassza.Dtos.Financial;
using Bassza.Features.GoogleSheets;
using Serilog;

namespace Bassza.Features.Reporting;

public static class DietaryExtensions
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
                        MedicalInformation = medicalInformation,
                        ParticipantEmail = dataModelParticipant.EmailPrimary
                    });
                    
                }
            }

        }

        return records;


    }
    
    public static List<OffsiteInfo> ProcessDietaries(this OlemsDataModel dataModel)
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

            
            foreach (var medicalInformation in dietaries)
            {
                records.Add(new OffsiteInfo()
                {
                    ParticipantId = dataModelParticipant.EventId,
                    ParticipantName = dataModelParticipant.Name,
                    MedicalInformation = medicalInformation,
                    ParticipantEmail = dataModelParticipant.EmailPrimary
                });
                    
            }

        }

        return records;

    }


    public static async Task UpdateOffsiteDietariesSheet(this SheetsApiManager apiManager, List<OffsiteInfo> report)
    {
        await Signals.Requestors.WaitAsync();
        if (!apiManager.IsActive) return;

        Log.Information("UpdateOffsiteDietariesSheet Start");
        
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
        
        Log.Information("UpdateOffsiteDietariesSheet End");
        Signals.ResetRequestor();
        
    }
    
    
    public static async Task UpdateOffsiteTourDietariesSheet(this SheetsApiManager apiManager, List<OffsiteInfo> reportIn)
    {
        await Signals.Requestors.WaitAsync();
        if (!apiManager.IsActive) return;

        Log.Information("UpdateOffsiteTourDietariesSheet Start");

        var filterList = new List<string>()
        {
            "Tamar Valley Wine Tour",
            "NW Food Tour",
            "Day Trip - Stanley",
            "NW Brewery Tour"
        };
        
        var report = reportIn
            .Where(pt => filterList.Any(lt => lt.Equals(pt.Activity.Name)))
            .OrderBy(pt => pt.Activity.Name)
            .ThenBy(pt => pt.Activity.Day)
            .ThenBy(pt => pt.ParticipantId)
             .ToList();
        
        var idList = report.Select(pt => pt.ParticipantId).Cast<object>().ToList();
        await apiManager.UpdateRow("A", "OffsiteDietaryAnonTours", idList, "Id");
        var offsiteList = report.Select(pt => pt.Activity.Name).Cast<object>().ToList();
        await apiManager.UpdateRow("B", "OffsiteDietaryAnonTours", offsiteList, "Activity");
        var dayList = report.Select(pt => pt.Activity.Day.DayOfWeek.ToString()).Cast<object>().ToList();
        await apiManager.UpdateRow("C", "OffsiteDietaryAnonTours", dayList, "Day");
        var allergyList = report.Select(pt => pt.MedicalInformation.Name).Cast<object>().ToList();
        await apiManager.UpdateRow("D", "OffsiteDietaryAnonTours", allergyList, "Dietary");
        
        Log.Information("UpdateOffsiteTourDietariesSheet End");
        Signals.ResetRequestor();
        
    }
    
    public static async Task UpdateDietariesSheet(this SheetsApiManager apiManager, List<OffsiteInfo> report)
    {
        await Signals.Requestors.WaitAsync();
        if (!apiManager.IsActive) return;
        
        Log.Information("UpdateDietariesSheet Start");
        
        report = report.OrderBy(pt => pt.ParticipantId).ToList();
        
        var idList = report.Select(pt => pt.ParticipantId).Cast<object>().ToList();
        await apiManager.UpdateRow("A", "FullDietary", idList, "Moot Id");
        
        var nameList = report.Select(pt => pt.ParticipantName).Cast<object>().ToList();
        await apiManager.UpdateRow("B", "FullDietary", nameList, "Name");
        
        var emailList = report.Select(pt => pt.ParticipantEmail).Cast<object>().ToList();
        await apiManager.UpdateRow("C", "FullDietary", emailList, "Email");

        var typeList = report.Select(pt => pt.MedicalInformation.MedicalInformationType.ToString())
            .Cast<object>().ToList();
        await apiManager.UpdateRow("D", "FullDietary", typeList, "Type");
        
        var allergyList = report.Select(pt => pt.MedicalInformation.Name).Cast<object>().ToList();
        await apiManager.UpdateRow("E", "FullDietary", allergyList, "Name");
        
        var reactionList = report.Select(pt => string.Concat(pt.MedicalInformation.Reaction, 
            pt.MedicalInformation.Information)).Cast<object>().ToList();
        await apiManager.UpdateRow("F", "FullDietary", reactionList, "Reaction/Information");
        
        var treatmentList = report.Select(pt => pt.MedicalInformation.Treatment).Cast<object>().ToList();
        await apiManager.UpdateRow("G", "FullDietary", treatmentList, "Treatment");
        
        Log.Information("UpdateDietariesSheet End");
        Signals.ResetRequestor();
        
    }
    
}   