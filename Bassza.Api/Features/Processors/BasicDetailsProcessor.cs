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

            if (preferredName.Length < 2) continue;
            
            dataModel.Participants
                .FirstOrDefault(pt => pt.EventId == idAsNo)!
                .NameFirst = preferredName;

        }
        
        dataModel.Participants = dataModel.Participants.OrderBy(pt => pt.EventId).ToList();
        
    }

        public static async Task ProcessTransportDetails(
        this OlemsDataModel dataModel, 
        string? overrideData = null, 
        bool saveDataForTest = false,
        bool consumeTestData = false)
    {        
        Log.Information("Getting Transport Details");

        var htmlData = "";

        var consumedTestData = false;
        
        if (consumeTestData)
        {
            if (File.Exists("transportDetails.html"))
            {
                htmlData = File.ReadAllText("transportDetails.html");
                consumedTestData = true;
            }
        }
        
        switch (consumedTestData)
        {
            case false when overrideData == null:
            {
                var reportRequest = new RequestDto()
                {
                    EndPointDto = Endpoints.TravelDetails
                };
        
                var medicalReportResponse = reportRequest.RunRequest();
                htmlData = await medicalReportResponse!.Content.ReadAsStringAsync();
                if (saveDataForTest) await File.WriteAllTextAsync("transportDetails.html", htmlData);
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
            
            var travelToMoot = participantInfo[23].InnerHtml.TrimFormatting();
            
            var travelFromMoot = participantInfo[25].InnerHtml.TrimFormatting();
            
            var pickupLocation = participantInfo[27].InnerHtml.TrimFormatting();

            dataModel.Participants
                .FirstOrDefault(pt => pt.EventId == idAsNo)!
                .TravelToMoot = travelToMoot;
            dataModel.Participants
                .FirstOrDefault(pt => pt.EventId == idAsNo)!
                .TravelFromMoot = travelFromMoot;
            dataModel.Participants
                .FirstOrDefault(pt => pt.EventId == idAsNo)!
                .PickupLocation = pickupLocation;

        }
        
        dataModel.Participants = dataModel.Participants.OrderBy(pt => pt.EventId).ToList();
        
    }
        
        public static async Task ProcessPayment(
        this OlemsDataModel dataModel, 
        string? overrideData = null, 
        bool saveDataForTest = false,
        bool consumeTestData = false)
    {        
        
        Log.Information("Getting Payment Report");
        
        var htmlData = "";
        
        var consumedTestData = false;
        
        if (consumeTestData)
        {
            if (File.Exists("payments.html"))
            {
                htmlData = File.ReadAllText("payments.html");
                consumedTestData = true;
            }
        }
        
        switch (consumedTestData)
        {
            case false when overrideData == null:
            {
                var reportRequest = new RequestDto()
                {
                    EndPointDto = Endpoints.Payments
                };
        
                var reportResponse = reportRequest.RunRequest();
                htmlData = await reportResponse!.Content.ReadAsStringAsync();
                if (saveDataForTest) await File.WriteAllTextAsync("payments.html", htmlData);
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

        bool foundFirstId = false;
        var currentId = 0;

        var currentFinancialPosition = new FinancialPosition();
        
        foreach (var node in table.ChildNodes)
        {
            var participantInfo = node.ChildNodes;
                
            if (participantInfo.Count < 2) continue;
            
            int idAsNo = 0;
            try
            {
                var id = participantInfo[1].InnerHtml.Split("<br>")[0].TrimFormatting();
                if (TryParse(id, out idAsNo))
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
                        .FinancialPosition = currentFinancialPosition;

                    currentFinancialPosition = new FinancialPosition();

                    currentId = idAsNo;
                }
                else
                {
                    if (!foundFirstId) continue;

                    var paymentsTable = node.ChildNodes[1].ChildNodes.FindFirst("table");

                    var dataResults = (paymentsTable.ChildNodes.Count - 3) / 2;

                    for (int i = 0; i < dataResults; i++)
                    {
                        var resultNo = 3 + i * 2;

                        var name = paymentsTable.ChildNodes[resultNo]
                            .ChildNodes[3].InnerText.TrimFormatting();
                        if (name.Contains("TOTALS:")) continue;
                        
                        var dueDate = paymentsTable.ChildNodes[resultNo]
                            .ChildNodes[5].InnerText.TrimFormatting();
                        var dueAmount = paymentsTable.ChildNodes[resultNo]
                            .ChildNodes[7].InnerText.TrimFormatting();
                        var receivedDate = paymentsTable.ChildNodes[resultNo]
                            .ChildNodes[9].InnerText.TrimFormatting();
                        var receivedValue = paymentsTable.ChildNodes[resultNo]
                            .ChildNodes[11].InnerText.TrimFormatting();
                        var note = paymentsTable.ChildNodes[resultNo]
                            .ChildNodes[13].InnerText.TrimFormatting();

                        if (dueDate.Length < 5) dueDate = "31-Dec-2022";
                        
                        var payment = new Payment()
                        {
                            PaymentName = name,
                            MootId = currentId,
                            DueDate = DateOnly.Parse(dueDate),
                            DueValue = dueAmount.TryParseToDouble(),
                            PaymentIdOrComment = note
                        };
                        
                        if (receivedDate.Length > 5)
                        {
                            payment.ReceivedDate = DateOnly.Parse(receivedDate);
                        }

                        if (payment.DueValue == 0)
                        {
                            payment.ReceivedDate = payment.DueDate;
                        }

                        if (receivedValue.Length > 3)
                        {
                            payment.ReceivedValue = receivedValue.TryParseToDouble();
                        }
                        
                        currentFinancialPosition.Payments.Add(payment);

                        if (payment.PaymentName.Equals("Payment 1"))
                        {
                            currentFinancialPosition.BaseFee += payment.DueValue;
                            if (!payment.IsOutstanding) 
                                currentFinancialPosition.Payment1Complete = true;
                            if (payment.ReceivedValue != null) 
                                currentFinancialPosition.Payment1 = (double)payment.ReceivedValue;
                        }
                        
                        if (payment.PaymentName.Equals("Payment 2"))
                        {
                            currentFinancialPosition.BaseFee += payment.DueValue;
                            if (!payment.IsOutstanding) 
                                currentFinancialPosition.Payment2Complete = true;
                            
                            if (payment.ReceivedValue != null) 
                                currentFinancialPosition.Payment2 = (double)payment.ReceivedValue;
                        }
                        
                        if (payment.PaymentName.Equals("Payment 3"))
                        {
                            currentFinancialPosition.BaseFee += payment.DueValue;
                            if (!payment.IsOutstanding) 
                                currentFinancialPosition.Payment3Complete = true;
                            
                            if (payment.ReceivedValue != null) 
                                currentFinancialPosition.Payment3 = (double)payment.ReceivedValue;
                        }
                        
                        if (payment.PaymentName.Equals("Expedition Payment 1"))
                        {
                            currentFinancialPosition.Expedition += payment.DueValue;
                            if (!payment.IsOutstanding) 
                                currentFinancialPosition.Expedition1Complete = true;
                            
                            if (payment.ReceivedValue != null) 
                                currentFinancialPosition.ExpeditionPayment1 = (double)payment.ReceivedValue;
                        }
                        
                        if (payment.PaymentName.Equals("Expedition Payment 2"))
                        {
                            currentFinancialPosition.Expedition += payment.DueValue;
                            if (!payment.IsOutstanding) 
                                currentFinancialPosition.Expedition2Complete = true;
                            
                            if (payment.ReceivedValue != null) 
                                currentFinancialPosition.ExpeditionPayment2 = (double)payment.ReceivedValue;
                        }

                        if (payment.PaymentName.Contains("Expedition Payment 3"))
                        {
                            currentFinancialPosition.Expedition += payment.DueValue;

                            var noPaymentThreeOutstanding = currentFinancialPosition
                                .Payments
                                .Where(py => py.PaymentName.Contains("Expedition Payment 3"))
                                .All(py => !py.IsOutstanding);
                            
                            currentFinancialPosition.Expedition3Complete = noPaymentThreeOutstanding;
                            
                            if (payment.ReceivedValue != null) 
                                currentFinancialPosition.ExpeditionPayment3 += (double)payment.ReceivedValue;
                        }
                        
                        if (payment.PaymentName.ToLower().Equals("tent fee"))
                        {
                            currentFinancialPosition.HaveTentPayment = true;
                            if (!payment.IsOutstanding) 
                                currentFinancialPosition.TentPaymentComplete = true;
                            
                        }
                        
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