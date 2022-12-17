using System.Collections.Immutable;
using Bassza.Api.Dtos;
using Bassza.Api.Dtos.Participant;
using Bassza.Dtos.Financial;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Serilog;

namespace Bassza.Features.GoogleSheets;

public class SheetsApiManager
{

    private Options _options;
    public bool IsActive { get; private set; } = false;
    private BaseClientService.Initializer? _initializer;
    public Spreadsheet Spreadsheet { get; private set; }

    private SheetsService _service;
    
    private string[] _scopes = { SheetsService.Scope.Spreadsheets};
    
    public SheetsApiManager(Options options)
    {
        _options = options;
    }

    public async Task TryConnect()
    {
        if (_options.GoogleSheetId == null) return;
        
        var credentialJsonSecrets = GoogleCredential.FromFile("apiKey.json").CreateScoped(_scopes);
        
        _initializer = new BaseClientService.Initializer()
        {
            ApplicationName = "Bassza",
            HttpClientInitializer = credentialJsonSecrets
        };

        Log.Information("Getting Sheets Access");

        _service = new SheetsService(_initializer);

        try
        {
            var sheet = await _service.Spreadsheets
                .Get(_options.GoogleSheetId).ExecuteAsync();
            if (sheet != null) Spreadsheet = sheet;
            IsActive = true;
        }
        catch (Exception e)
        {
            Log.Error("Could not access sheet.");
        }

    }

    public async Task UpdateDataModel(OlemsDataModel dataModel)
    {
        await Signals.Requestors.WaitAsync();
        if (!IsActive) return;
        Log.Information("UpdateDataModel Start");
        
        var idList = dataModel.Participants.Select(value => value.EventId).Cast<object>().ToList();
        await UpdateRow("A", "Participants", idList, "Id");
        
        var contingentList = dataModel.Participants.Select(value => value.Contingent).Cast<object>().ToList();
        await UpdateRow("B", "Participants", contingentList, "Contingent");
        
        var nameList = dataModel.Participants.Select(value => value.Name).Cast<object>().ToList();
        await UpdateRow("C", "Participants", nameList, "Name");
        
        var ageList = dataModel.Participants.Select(value => value.Age.Days/365).Cast<object>().ToList();
        await UpdateRow("D", "Participants", ageList, "Age");
        
        var emailList = dataModel.Participants.Select(value => value.EmailPrimary).Cast<object>().ToList();
        await UpdateRow("E", "Participants", emailList, "Email");

        var payedBase1List = dataModel.Participants.Select(value => value.FinancialPosition.Payment1Complete).Cast<object>().ToList();
        await UpdateRow("F", "Participants", payedBase1List, "Base 1");
        var payedBase2List = dataModel.Participants.Select(value => value.FinancialPosition.Payment2Complete).Cast<object>().ToList();
        await UpdateRow("G", "Participants", payedBase2List, "Base 2");
        var payedBase3List = dataModel.Participants.Select(value => value.FinancialPosition.Payment3Complete).Cast<object>().ToList();
        await UpdateRow("H", "Participants", payedBase3List, "Base 3");

        var baseFeeSumList = dataModel.Participants.Select(value => value.FinancialPosition.BaseFeeCompletedSum).Cast<object>().ToList();
        await UpdateRow("I", "Participants", baseFeeSumList, "Base Paid");
        var baseFeeOwed = dataModel.Participants.Select(value => value.FinancialPosition.BaseFeeOwed).Cast<object>().ToList();
        await UpdateRow("J", "Participants", baseFeeOwed, "Base Owed");
        
        var statusList = dataModel.Participants.Select(value => value.Status).Cast<object>().ToList();
        await UpdateRow("K", "Participants", statusList, "Status"); 
        
        var ppList = dataModel.Participants.Select(value => value.PayingParticipant).Cast<object>().ToList();
        await UpdateRow("L", "Participants", ppList, "PayingParticipant"); 
       
        
        var payedExped1List = dataModel.Participants.Select(value => value.FinancialPosition.Expedition1Complete).Cast<object>().ToList();
        await UpdateRow("M", "Participants", payedExped1List, "Exped 1");
        
        var payedExped2List = dataModel.Participants.Select(value => value.FinancialPosition.Expedition2Complete).Cast<object>().ToList();
        await UpdateRow("N", "Participants", payedExped2List, "Exped 2");

        var payedExped3List = dataModel.Participants.Select(value => value.FinancialPosition.Expedition3Complete).Cast<object>().ToList();
        await UpdateRow("O", "Participants", payedExped2List, "Exped 3");
        
        var expedFeeSumList = dataModel.Participants.Select(value => value.FinancialPosition.ExpeditionFeeCompletedSum).Cast<object>().ToList();
        await UpdateRow("P", "Participants", expedFeeSumList, "Exped Paid");
        var expedFeeOwed = dataModel.Participants.Select(value => value.FinancialPosition.ExpeditionFeeOwed).Cast<object>().ToList();
        await UpdateRow("Q", "Participants", expedFeeOwed, "Exped Owed");
        
        // var payedExped3List = dataModel.Participants.Select(value => value.FinancialPosition.Expedition3Complete).Cast<object>().ToList();
        // await UpdateRow("O", "Participants", payedExped3List, "Exped 3");
        
        // var  = dataModel.Participants.Select(value => value.EmailPrimary).Cast<object>().ToList();
        // await UpdateRow("E", "Participants", emailList, "Email");

        
        Log.Information("UpdateDataModel End");
        Signals.ResetRequestor();
        
    }


    public async Task UpdateFinancialPosition(IntegratedPosition position)
    {
        await Signals.Requestors.WaitAsync();
        Log.Information("UpdateFinancialPosition Start");
        var labelPosition = new List<object>()
        {
            "",
            "No Payment",
            "1st Payment",
            "2nd Payment",
            "3rd Payment",
            "Total",
            "",
            "Total Paid",
            "Total Owing"
        };
        await UpdateRow("B", "Financial", labelPosition, "");
        
        var staffPosition = new List<object>()
        {
            "",
            position.StaffBasePayment.NoPaymentCount,
            position.StaffBasePayment.Payment1Count,
            position.StaffBasePayment.Payment2Count,
            position.StaffBasePayment.Payment3Count,
            position.StaffBasePayment.Participants,
            "",
            position.StaffBasePayment.TotalPaid,
            position.StaffBasePayment.TotalOwed
        };
        await UpdateRow("C", "Financial", staffPosition, "Staff (Base)");
        
        var fullFeePosition = new List<object>()
        {
            "",
            position.FullFeeBasePayment.NoPaymentCount,
            position.FullFeeBasePayment.Payment1Count,
            position.FullFeeBasePayment.Payment2Count,
            position.FullFeeBasePayment.Payment3Count,
            position.FullFeeBasePayment.Participants,
            "",
            position.FullFeeBasePayment.TotalPaid,
            position.FullFeeBasePayment.TotalOwed
        };
        await UpdateRow("D", "Financial", fullFeePosition, "Full Fee (Base)");
        
        var staffExped = new List<object>()
        {
            "",
            position.StaffExpeditionPayment.NoPaymentCount,
            position.StaffExpeditionPayment.Payment1Count,
            position.StaffExpeditionPayment.Payment2Count,
            position.StaffExpeditionPayment.Payment3Count,
            position.StaffBasePayment.Participants,
            "",
            position.StaffExpeditionPayment.TotalPaid,
            position.StaffExpeditionPayment.TotalOwed
        };
        await UpdateRow("G", "Financial", staffExped, "Staff (Exped)");
        
        var fullFeeExped = new List<object>()
        {
            "",
            position.FullFeeExpeditionPayment.NoPaymentCount,
            position.FullFeeExpeditionPayment.Payment1Count,
            position.FullFeeExpeditionPayment.Payment2Count,
            position.FullFeeExpeditionPayment.Payment3Count,
            position.FullFeeBasePayment.Participants,
            "",
            position.FullFeeExpeditionPayment.TotalPaid,
            position.FullFeeExpeditionPayment.TotalOwed
        };
        await UpdateRow("H", "Financial", fullFeeExped, "Full Fee (Exped)");

        Log.Information("UpdateFinancialPosition End");
        Signals.ResetRequestor();

    }
    

    public async Task UpdateRow(
        string column, 
        string sheetName, 
        List<object> values, 
        string title = "Undefined")
    {
        await SetColumnName(column, sheetName, title);
        
        var lastParticipant = values.Count;
        var sheetRange = $"{sheetName}!{column}{2}:{column}{lastParticipant + 2}";
        
        var valueRange = new ValueRange();
        valueRange.MajorDimension = "COLUMNS";
        valueRange.Values = new List<IList<object>>{ values };       
        
        var updateRequest = _service.Spreadsheets
            .Values.Update(valueRange, _options.GoogleSheetId, sheetRange);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        var result = await updateRequest.ExecuteAsync();
    }

    
    public async Task UpdateDebitedPayments(List<Payment> payments)
    {
        payments.Sort((x, y)
                => x.ReceivedDate!.Value.CompareTo(y.ReceivedDate!.Value));
        
        var dateList = payments.Select(pt => ((DateOnly)pt.ReceivedDate!)
            .ToString("yyyy/MM/dd")).Cast<object>().ToList();
        
        var valueList = payments.Select(pt => pt.ReceivedValue)
            .Cast<object>().ToList();
        
        var itemList = payments.Select(pt => pt.PaymentName)
            .Cast<object>().ToList();
        
        await Signals.Requestors.WaitAsync();

        if (!IsActive) return;
        Log.Information("UpdateDebitedPayments Start");
        
        await UpdateRow("A", "DebitedPayments", dateList, "Received");
        await UpdateRow("B", "DebitedPayments", valueList, "Value");
        await UpdateRow("C", "DebitedPayments", itemList, "Item");

        Log.Information("UpdateDebitedPayments End");
        Signals.ResetRequestor();
        
    }
    
    private async Task SetColumnName(
        string column, 
        string sheetName,
        string title)
    {
        var sheetRange = $"{sheetName}!{column}{1}";
        
        var valueRange = new ValueRange();
        valueRange.MajorDimension = "COLUMNS";
        
        var obList = new List<object> { title };
        valueRange.Values = new List<IList<object>>{ obList };        
        
        var updateRequest = _service.Spreadsheets
            .Values.Update(valueRange, _options.GoogleSheetId, sheetRange);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        
        var result = await updateRequest.ExecuteAsync();
    }
    
}