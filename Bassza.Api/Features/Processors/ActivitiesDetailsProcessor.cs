using System.Globalization;
using Bassza.Api.Dtos.Participant;
using Bassza.Api.Extensions;
using HtmlAgilityPack;
using Serilog;

namespace Bassza.Api.Features.Processors;

public static class ActivitiesDetailsProcessor
{
            
    public static async Task ProcessOffsiteActivies(
        this OlemsDataModel dataModel, 
        string? overrideData = null, 
        bool saveDataForTest = false,
        bool consumeTestData = false)
    {        
        Log.Information("Getting Activities Report");
        
        var htmlData = "";
        
        var consumedTestData = false;
        
        if (consumeTestData)
        {
            if (File.Exists("activitiesReport.html"))
            {
                htmlData = File.ReadAllText("activitiesReport.html");
                consumedTestData = true;
            }
        }
        
        switch (consumedTestData)
        {
            case false when overrideData == null:
            {
                var reportRequest = new RequestDto()
                {
                    EndPointDto = Endpoints.OffSiteActivities
                };
        
                var reportResponse = reportRequest.RunRequest();
                htmlData = await reportResponse!.Content.ReadAsStringAsync();
                if (saveDataForTest) await File.WriteAllTextAsync("activitiesReport.html", htmlData);
                break;
            }
            case false:
                htmlData = overrideData;
                break;
        }
        
        var htmlDoc = new HtmlDocument();
        
        htmlDoc.LoadHtml(htmlData);

        var table = htmlDoc.DocumentNode
            .ChildNodes.FindFirst("table");
        
        Log.Debug($"Found {table.ChildNodes.Count} html nodes");
        
        try
        {
            foreach (var node in table.ChildNodes)
            {
                var participantInfo = node.ChildNodes;
                
                if (participantInfo.Count < 2) continue;
            
                int idAsNo = 0;
                try
                {
                    var id = participantInfo[1].InnerHtml.Split(",")[0].TrimFormatting();
                    if (!int.TryParse(id, out idAsNo)) continue;
                }
                catch (Exception e)
                {
                    Log.Warning($"Error {e.Message}");
                    continue;
                }

                var activityName = participantInfo[9].InnerHtml.TrimFormatting();
                var day = participantInfo[13].InnerHtml.TrimFormatting().Split("\r\n\t\t\t\t\t\t")[1];
                var session = participantInfo[15].InnerHtml.TrimFormatting();
                var cost = participantInfo[17].InnerHtml.TrimFormatting();


                var sessionType = ActivitySession.Unknown;

                if (session.Contains("Full Day")) sessionType = ActivitySession.FullDay;
                if (session.Contains("Morning")) sessionType = ActivitySession.Morning;
                if (session.Contains("Afternoon")) sessionType = ActivitySession.Afternoon;
                
                dataModel
                    .Participants
                    .FirstOrDefault(pt => pt.EventId.Equals(idAsNo))!
                    .OffsiteActivities.Add(new OffsiteActivity()
                    {
                        Name = activityName,
                        Day = DateOnly.Parse(day, new CultureInfo("en-US")),
                        Cost = double.Parse(cost),
                        Session = sessionType
                    });
                
            }
        }
        catch (Exception e)
        { 
            Log.Error(e.Message);
        }

    }
    
        public static async Task ProcessOffsiteExpeditions(
        this OlemsDataModel dataModel, 
        string? overrideData = null, 
        bool saveDataForTest = false,
        bool consumeTestData = false)
    {        
        Log.Information("Getting Activities Report");
        
        var htmlData = "";
        
        var consumedTestData = false;
        
        if (consumeTestData)
        {
            if (File.Exists("expeditionsReport.html"))
            {
                htmlData = File.ReadAllText("expeditionsReport.html");
                consumedTestData = true;
            }
        }
        
        switch (consumedTestData)
        {
            case false when overrideData == null:
            {
                var reportRequest = new RequestDto()
                {
                    EndPointDto = Endpoints.ExpeditionAllocation
                };
        
                var reportResponse = reportRequest.RunRequest();
                htmlData = await reportResponse!.Content.ReadAsStringAsync();
                if (saveDataForTest) await File.WriteAllTextAsync("expeditionsReport.html", htmlData);
                break;
            }
            case false:
                htmlData = overrideData;
                break;
        }
        
        var htmlDoc = new HtmlDocument();
        
        htmlDoc.LoadHtml(htmlData);

        var table = htmlDoc.DocumentNode
            .ChildNodes.FindFirst("table");
        
        Log.Debug($"Found {table.ChildNodes.Count} html nodes");
        
        try
        {
            foreach (var node in table.ChildNodes)
            {
                var participantInfo = node.ChildNodes;
                
                if (participantInfo.Count < 2) continue;
            
                int idAsNo = 0;
                try
                {
                    var id = participantInfo[1].InnerHtml.Split(",")[0].TrimFormatting();
                    if (!int.TryParse(id, out idAsNo)) continue;
                }
                catch (Exception e)
                {
                    Log.Warning($"Error {e.Message}");
                    continue;
                }

                var expeditionRaw = participantInfo[15].InnerHtml.TrimFormatting();;

                if (!expeditionRaw.Contains('-')) continue;

                var brokenExpedition = expeditionRaw.Split("-");

                dataModel
                    .Participants
                    .FirstOrDefault(pt => pt.EventId.Equals(idAsNo))!
                    .Expedition = brokenExpedition[0];
                
                dataModel
                    .Participants
                    .FirstOrDefault(pt => pt.EventId.Equals(idAsNo))!
                    .ExpeditionUnit = brokenExpedition[1];

            }
        }
        catch (Exception e)
        { 
            Log.Error(e.Message);
        }

    }
    
}