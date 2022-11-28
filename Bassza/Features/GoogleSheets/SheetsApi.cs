using Bassza.Api.Dtos;
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
        if (!IsActive) return;
        
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


    }

    private async Task UpdateRow(
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