using Bassza.Api.Dtos.Participant;
using Bassza.Api.Extensions;
using HtmlAgilityPack;
using Serilog;
using static System.Int32;

namespace Bassza.Api.Features.Processors;

public static class BasicDetailsProcessor
{
    public static async Task ProcessBasicDetails(
        this OlemsDataModel dataModel, 
        string? overrideData = null, 
        bool saveDataForTest = false,
        bool consumeTestData = false)
    {        
        Log.Information("Getting Basic Details");

        var htmlData = "";

        var consumedTestData = false;
        
        if (consumeTestData)
        {
            if (File.Exists("generalData.html"))
            {
                htmlData = File.ReadAllText("generalData.html");
                consumedTestData = true;
            }
        }
        
        switch (consumedTestData)
        {
            case false when overrideData == null:
            {
                var reportRequest = new RequestDto()
                {
                    EndPointDto = Endpoints.BasicDetailsWithEmails
                };
        
                var medicalReportResponse = reportRequest.RunRequest();
                htmlData = await medicalReportResponse!.Content.ReadAsStringAsync();
                if (saveDataForTest) await File.WriteAllTextAsync("generalData.html", htmlData);
                break;
            }
            case false:
                htmlData = overrideData;
                break;
        }
        
        var htmlDoc = new HtmlDocument();
        
        htmlDoc.LoadHtml(htmlData);

        var table = htmlDoc.DocumentNode
            .ChildNodes.FindFirst("html")
            .ChildNodes.FindFirst("body")
            .ChildNodes.FindFirst("form")
            .ChildNodes.FindFirst("table");

        foreach (var node in table.ChildNodes)
        {
            var participantDto = new Participant();
            
            var participantInfo = node.ChildNodes;
            if (participantInfo.Count < 2) continue;
            
            int idAsNo = 0;
            try
            {
                var id = participantInfo[1].InnerHtml.TrimFormatting();
                if (!TryParse(id, out idAsNo)) continue;
            }
            catch (Exception e)
            {
                Log.Warning($"Error {e.Message}");
                continue;
            }
            
            participantDto.EventId = idAsNo;
            
            participantDto.NameLast = participantInfo[3].InnerHtml.TrimFormatting();
            participantDto.NameFirst = participantInfo[5].InnerHtml.TrimFormatting();
            
            var role = participantInfo[7].InnerHtml.TrimFormatting();
            participantDto.Contingent = participantInfo[9].InnerHtml.TrimFormatting();
            
            var emailPrimary = participantInfo[15].InnerHtml.TrimFormatting();
            participantDto.EmailPrimary = emailPrimary;
            var emailSecondary = participantInfo[17].InnerHtml.TrimFormatting();
            participantDto.EmailSecondary = emailSecondary;
            
            var status = participantInfo[19].InnerHtml.TrimFormatting();
            participantDto.Status = status;

            var dateOfBirth = participantInfo[23].InnerHtml.TrimFormatting();
            participantDto.DateOfBirth = DateTime.Parse(dateOfBirth);

            dataModel.Participants.Add(participantDto);
            
        }
        
        dataModel.Participants = dataModel.Participants.OrderBy(pt => pt.EventId).ToList();
        
    }
    
        public static async Task ProcessMailingDetails(
        this OlemsDataModel dataModel, 
        string? overrideData = null, 
        bool saveDataForTest = false,
        bool consumeTestData = false)
    {        
        Log.Information("Getting Mailing Details");

        var htmlData = "";

        var consumedTestData = false;
        
        if (consumeTestData)
        {
            if (File.Exists("mailingDetails.html"))
            {
                htmlData = File.ReadAllText("mailingDetails.html");
                consumedTestData = true;
            }
        }
        
        switch (consumedTestData)
        {
            case false when overrideData == null:
            {
                var reportRequest = new RequestDto()
                {
                    EndPointDto = Endpoints.MailingLabels
                };
        
                var medicalReportResponse = reportRequest.RunRequest();
                htmlData = await medicalReportResponse!.Content.ReadAsStringAsync();
                if (saveDataForTest) await File.WriteAllTextAsync("mailingDetails.html", htmlData);
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

        foreach (var node in table.ChildNodes)
        {
            var participantInfo = node.ChildNodes;
            if (participantInfo.Count < 2) continue;
            
            int idAsNo = 0;
            try
            {
                var id = participantInfo[1].InnerHtml.TrimFormatting();
                if (!TryParse(id, out idAsNo)) continue;
            }
            catch (Exception e)
            {
                Log.Warning($"Error {e.Message}");
                continue;
            }
            
            var preferredName = participantInfo[11].InnerHtml.TrimFormatting();

            dataModel.Participants
                .FirstOrDefault(pt => pt.EventId == idAsNo)!
                .NameFirst = preferredName;

        }
        
        dataModel.Participants = dataModel.Participants.OrderBy(pt => pt.EventId).ToList();
        
    }
    
    public static async Task ProcessPayments(
        this OlemsDataModel dataModel, 
        string? overrideData = null, 
        bool saveDataForTest = false,
        bool consumeTestData = false)
    {        
        Log.Information("Getting Payment Details Extended");
        
        var htmlData = "";
        
        var consumedTestData = false;
        
        if (consumeTestData)
        {
            if (File.Exists("paymentData.html"))
            {
                htmlData = File.ReadAllText("paymentData.html");
                consumedTestData = true;
            }
        }
        
        switch (consumedTestData)
        {
            case false when overrideData == null:
            {
                var reportRequest = new RequestDto()
                {
                    EndPointDto = Endpoints.PaymentDetailsExtended
                };
        
                var reportResponse = reportRequest.RunRequest();
                htmlData = await reportResponse!.Content.ReadAsStringAsync();
                if (saveDataForTest) await File.WriteAllTextAsync("paymentData.html", htmlData);
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
                    var id = participantInfo[1].InnerHtml.TrimFormatting();
                    if (!TryParse(id, out idAsNo)) continue;
                }
                catch (Exception e)
                {
                    Log.Warning($"Error {e.Message}");
                    continue;
                }
                
                var status = participantInfo[3].InnerHtml.TrimFormatting();
                var contingent = participantInfo[5].InnerHtml.TrimFormatting();
                var nameWLink = participantInfo[7].InnerHtml.TrimFormatting();
                var role = participantInfo[9].InnerHtml.TrimFormatting();
                var unit = participantInfo[11].InnerHtml.TrimFormatting();
                var baseFee = participantInfo[13].InnerHtml.TrimFormatting();
                var expedition = participantInfo[14].InnerHtml.TrimFormatting();
                var offsiteHolds = participantInfo[16].InnerHtml.TrimFormatting();
                var due = participantInfo[18].InnerHtml.TrimFormatting();
                var paid = participantInfo[20].InnerHtml.TrimFormatting();
                var expeditionPayment1 = participantInfo[22].InnerHtml.TrimFormatting();
                var expeditionPayment2 = participantInfo[24].InnerHtml.TrimFormatting();
                var expeditionPayment3Tba = participantInfo[26].InnerHtml.TrimFormatting();
                var noMootFee = participantInfo[28].InnerHtml.TrimFormatting();
                var payment1 = participantInfo[30].InnerHtml.TrimFormatting();
                var payment2 = participantInfo[32].InnerHtml.TrimFormatting();
                var payment3 = participantInfo[34].InnerHtml.TrimFormatting();
                var offSitePayment = participantInfo[36].InnerHtml.TrimFormatting();
                var offSiteHold = participantInfo[38].InnerHtml.TrimFormatting();
                var otherPayment = participantInfo[40].InnerHtml.TrimFormatting();
                var refunds = participantInfo[42].InnerHtml.TrimFormatting();
                var outstanding = participantInfo[44].InnerHtml.TrimFormatting();

                var financialPosition = new FinancialPosition();
                
                financialPosition.BaseFee = baseFee.TryParseToDouble();
                financialPosition.Expedition = expedition.TryParseToDouble();
                financialPosition.OffsiteHolds = offsiteHolds.TryParseToDouble();
                financialPosition.Due = due.TryParseToDouble();
                financialPosition.Paid = paid.TryParseToDouble();
                financialPosition.ExpeditionPayment1 = expeditionPayment1.TryParseToDouble();
                financialPosition.ExpeditionPayment2 = expeditionPayment2.TryParseToDouble();
                financialPosition.ExpeditionPayment3Tba = expeditionPayment3Tba.TryParseToDouble();
                financialPosition.NoMootFee = noMootFee.TryParseToDouble();
                financialPosition.Payment1 = payment1.TryParseToDouble();
                financialPosition.Payment2 = payment2.TryParseToDouble();
                financialPosition.Payment3 = payment3.TryParseToDouble();
                financialPosition.OffSitePayment = offSitePayment.TryParseToDouble();
                financialPosition.OffSiteHold = offSiteHold.TryParseToDouble();
                financialPosition.OtherPayment = otherPayment.TryParseToDouble();
                financialPosition.Refunds = refunds.TryParseToDouble();
                financialPosition.Outstanding = outstanding.TryParseToDouble();

                var participantExists = dataModel
                    .Participants
                    .FirstOrDefault(pt => pt.EventId.Equals(idAsNo));
                
                if (participantExists == null || participantExists.EventId != idAsNo)
                {
                    Log.Warning($"Could not find user by id {idAsNo}");
                }
                
                dataModel
                    .Participants
                    .FirstOrDefault(pt => pt.EventId.Equals(idAsNo))!
                    .FinancialPosition = financialPosition;
                
            }
        }
        catch (Exception e)
        { 
            Log.Error(e.Message);
        }

    }

}