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

        var consumeTestData = true;
        
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
        var offsiteFlagged = dataModel.Participants.Where(pt => pt.OffsiteDiscrepancy);
        var fullDietary = dataModel.ProcessDietaries();
        var offSiteDietaryReport = dataModel.ProcessOffsiteDietaries();

        var updateTasks = new List<Task>();
        
        await Task.Delay(15000);

        updateTasks.Add(_sheetsApiManager.UpdateFinancialPosition(model));
        updateTasks.Add(_sheetsApiManager.UpdateDataModel(dataModel));
        updateTasks.Add(_sheetsApiManager.UpdateDietariesSheet(fullDietary));
        updateTasks.Add(_sheetsApiManager.UpdateOffsiteDietariesSheet(offSiteDietaryReport));
        updateTasks.Add(_sheetsApiManager.UpdateOffsiteFullDaySheet(dataModel));
        
        foreach (var updateTask in updateTasks)
        {
            await updateTask.WaitAsync(new CancellationToken());
        }

    }
    
}