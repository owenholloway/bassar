using Bassza.Api.Dtos;
using Bassza.Api.Features;
using Bassza.Api.Features.Processors;
using Bassza.Features;
using Bassza.Features.GoogleSheets;
using Google.Apis.Sheets.v4.Data;
using Serilog;

namespace Bassza;

public class Main
{

    private readonly Signals _signals;
    private readonly Options _options;
    private readonly SheetsApiManager _sheetsApiManager;

    public Main(Signals signals, 
        ILogger logger, 
        Options options, 
        SheetsApiManager sheetsApiManager)
    {
        _signals = signals;
        _options = options;
        _sheetsApiManager = sheetsApiManager;
    }

    public async Task Run()
    {
        Log.Information("Starting OLEMs Processor");

        var olemsSession = Session.Create(new LoginDetailsDto()
        {
            Username = _options.Username,
            Password = _options.Password
        });

        var loggedIn = olemsSession.LoginOpen();

        if (!loggedIn)
        {
            Log.Error("Incorrect Login Details");
            return;
        }
        
        Log.Information("Logged In");

        await Task.Delay(200);

        var dataModel = new OlemsDataModel();
        await Task.Delay(200);
        await dataModel.ProcessBasicDetails(saveDataForTest: true, consumeTestData:true);
        await Task.Delay(200);
        await dataModel.ProcessPayments(saveDataForTest: true, consumeTestData:true);
        await Task.Delay(200);
        await dataModel.ProcessMedicalReportResponse(saveDataForTest: true, consumeTestData:true);
        await Task.Delay(200);
        await dataModel.ProcessOffsiteActivies(saveDataForTest: true, consumeTestData: true);
        
        var model = dataModel.CalculatePosition();

        var offsiteFlagged = dataModel.Participants.Where(pt => pt.OffsiteDiscrepancy);

        await _sheetsApiManager.UpdateDataModel(dataModel);

    }
    
}