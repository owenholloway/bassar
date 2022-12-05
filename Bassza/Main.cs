using System.Security.Authentication;
using Bassza.Api.Dtos;
using Bassza.Api.Features;
using Bassza.Api.Features.Processors;
using Bassza.Features;
using Bassza.Features.GoogleSheets;
using Bassza.Features.Reporting;
using Google.Apis.Sheets.v4.Data;
using Serilog;

namespace Bassza;

public class Main
{

    private readonly Signals _signals;
    private readonly Options _options;
    private readonly SheetsApiManager _sheetsApiManager;
    private TotpManager _totpManager;

    public Main(Signals signals, 
        ILogger logger, 
        Options options, 
        SheetsApiManager sheetsApiManager, 
        TotpManager totpManager)
    {
        _signals = signals;
        _options = options;
        _sheetsApiManager = sheetsApiManager;
        _totpManager = totpManager;
    }

    public async Task Run()
    {
        Log.Information("Starting OLEMs Processor");

        var olemsSession = Session.Create(new LoginDetailsDto()
        {
            Username = _options.Username,
            Password = _options.Password
        });

        var consumeTestData = false;
        
        var loggedIn = consumeTestData;
        
        if (!loggedIn) loggedIn = olemsSession.LoginOpen(_totpManager.GetTotp());

        if (!loggedIn)
        {
            Log.Error("Incorrect Login Details");
            throw new AuthenticationException("Login failed");
        }
        
        Log.Information("Logged In");
        
        await Task.Delay(200);

        var dataModel = new OlemsDataModel();
        await Task.Delay(200);
        await dataModel.ProcessBasicDetails(saveDataForTest: true, consumeTestData: consumeTestData);
        await Task.Delay(200);
        await dataModel.ProcessMailingDetails(saveDataForTest: true, consumeTestData: consumeTestData);
        await Task.Delay(200);
        await dataModel.ProcessPayments(saveDataForTest: true, consumeTestData: consumeTestData);
        await Task.Delay(200);
        await dataModel.ProcessMedicalReportResponse(saveDataForTest: true, consumeTestData: consumeTestData);
        await Task.Delay(200);
        await dataModel.ProcessOffsiteActivies(saveDataForTest: true, consumeTestData: consumeTestData);
        
        var model = dataModel.CalculatePosition();
        await Task.Delay(1500);
        await _sheetsApiManager.UpdateFinancialPosition(model);
        
        var offsiteFlagged = dataModel.Participants.Where(pt => pt.OffsiteDiscrepancy);
        await Task.Delay(1500);
        await _sheetsApiManager.UpdateDataModel(dataModel);


        var offSiteDietaryReport = dataModel.ProcessOffsiteDietaries();
        await Task.Delay(1500);
        await _sheetsApiManager.UpdateOffsiteDietariesSheet(offSiteDietaryReport);
        
        await Task.Delay(1500);
        await _sheetsApiManager.UpdateOffsiteFullDaySheet(dataModel);


    }
    
}