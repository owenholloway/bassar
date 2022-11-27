using Bassza.Api.Dtos.Participant;
using Bassza.Api.Extensions;
using HtmlAgilityPack;
using Serilog;

namespace Bassza.Api.Features.Processors;

public static class MedicalReportProcessor
{
    public static async Task ProcessMedicalReportResponse(
        this OlemsDataModel dataModel, 
        string? overrideData = null, 
        bool saveDataForTest = false,
        bool consumeTestData = false)
    {        
        
        Log.Information("Getting Medical Report");
        
        var htmlData = "";
        
        var consumedTestData = false;
        
        if (consumeTestData)
        {
            if (File.Exists("medical.html"))
            {
                htmlData = File.ReadAllText("medical.html");
                consumedTestData = true;
            }
        }
        
        switch (consumedTestData)
        {
            case false when overrideData == null:
            {
                var reportRequest = new RequestDto()
                {
                    EndPointDto = Endpoints.ReportMedical
                };
        
                var reportResponse = reportRequest.RunRequest();
                htmlData = await reportResponse!.Content.ReadAsStringAsync();
                if (saveDataForTest) await File.WriteAllTextAsync("medical.html", htmlData);
                break;
            }
            case false:
                htmlData = overrideData;
                break;
        }
        
        var htmlDoc = new HtmlDocument();
        
        htmlDoc.LoadHtml(htmlData);

        var table = htmlDoc.DocumentNode
            .ChildNodes.FindFirst("form")
            .ChildNodes.FindFirst("table");

        bool foundFirstId = false;
        var currentId = 0;

        var currentMedInfo = new List<MedicalInformation>();
        
        foreach (var node in table.ChildNodes)
        {
            var participantInfo = node.ChildNodes;
                
            if (participantInfo.Count < 2) continue;
            
            int idAsNo = 0;
            try
            {
                var id = participantInfo[1].InnerHtml.Split("<br>")[0].TrimFormatting();
                if (int.TryParse(id, out idAsNo))
                {
                    if (!foundFirstId)
                    {
                        foundFirstId = true;
                        currentId = idAsNo;
                        continue;
                    }
                    
                    dataModel
                        .Participants
                        .FirstOrDefault(pt => pt.EventId.Equals(currentId))!
                        .MedicalInformation = currentMedInfo;

                    currentMedInfo = new List<MedicalInformation>();

                    currentId = idAsNo;
                }
                else
                {
                    if (!foundFirstId) continue;

                    if (node.ChildNodes.Count < 4) continue;
                    
                    var tableHtml = new HtmlDocument();
        
                    tableHtml.LoadHtml(node.InnerHtml);

                    var dataTable = tableHtml.DocumentNode.
                        ChildNodes[3].ChildNodes.FindFirst("table");
                    
                    var infoSize = 1;
                    var medItem = new MedicalInformation();
                    
                    var type = dataTable.ChildNodes[1]
                        .ChildNodes[1].InnerText;

                    if (type.Contains("Medical Conditions"))
                    {
                        medItem.MedicalInformationType = MedicalInformationType.MedicalCondition;
                        infoSize = 2;
                    }

                    if (type.Contains("Medications"))
                    {
                        medItem.MedicalInformationType = MedicalInformationType.Medication;
                        infoSize = 3;
                    }
                    
                    if (type.Contains("Medical Aids"))
                    {
                        medItem.MedicalInformationType = MedicalInformationType.MedicalAid;
                        infoSize = 2;
                    }
                    
                    if (type.Contains("Allergies"))
                    {
                        medItem.MedicalInformationType = MedicalInformationType.Allergies;
                        infoSize = 3;
                    }

                    if (type.Contains("Diet"))
                    {
                        medItem.MedicalInformationType = MedicalInformationType.DietaryRequirements;
                        infoSize = 3;
                    }
                    
                    var dataResults = (dataTable.ChildNodes.Count - 3) / 2;

                    for (int i = 0; i < dataResults; i++)
                    {
                        var resultNo = 3 + i * 2;

                        var name = dataTable.ChildNodes[resultNo]
                            .ChildNodes[3].InnerText;

                        var extra1 = "";
                        var extra2 = "";

                        
                        if (infoSize >= 2)
                        {
                            extra1 =
                                dataTable.ChildNodes[resultNo]
                                    .ChildNodes[5].InnerText;
                        }
                        
                        if (infoSize >= 3)
                        {
                            extra2 =
                                dataTable.ChildNodes[resultNo]
                                    .ChildNodes[7].InnerText;
                        }

                        medItem.Name = name;
                        
                        switch (medItem.MedicalInformationType)
                        {   
                            case MedicalInformationType.MedicalCondition:
                                medItem.FurtherInformation = extra1;
                                break;
                            
                            case MedicalInformationType.Medication:
                                medItem.Dosage = extra1;
                                medItem.MethodOfAdministration = extra2;
                                break;
                            
                            case MedicalInformationType.MedicalAid:
                                medItem.Reason = extra1;
                                break;
                            
                            case MedicalInformationType.Allergies:
                                medItem.Reaction = extra1;
                                medItem.Treatment = extra2;
                                break;
                            
                            case MedicalInformationType.DietaryRequirements:
                                medItem.DietCode = extra1;
                                medItem.Information = extra2;
                                break;
                        }

                        var newMedItem = new MedicalInformation
                        {
                            MedicalInformationType = medItem.MedicalInformationType
                        };

                        currentMedInfo.Add(medItem);
                        medItem = newMedItem;
                        
                    }
                    
                }
                
            }
            catch (Exception e)
            {
                Log.Warning($"Error {e.Message}");
                continue;
            }

        }
        
    }
}