using Bassza.Api.Dtos;
using Bassza.Api.Features;
using Bassza.Api.Features.Processors;
using Bassza.Features;
using Serilog;

namespace Bassza;

public class Main
{

    private readonly Signals _signals;
    private readonly ILogger _logger;
    private readonly Options _options;

    public Main(Signals signals, 
        ILogger logger, 
        Options options)
    {
        _signals = signals;
        _logger = logger;
        _options = options;
    }

    public async Task Run()
    {
        
        Log.Logger = _logger;
        
        _logger.Information("Starting OLEMs Processor");

        var olemsSession = Session.Create(new LoginDetailsDto()
        {
            Username = _options.Username,
            Password = _options.Password
        });

        var loggedIn = olemsSession.LoginOpen();

        if (!loggedIn)
        {
            _logger.Error("Incorrect Login Details");
            _signals.ApplicationDone.Set();
        }
        
        _logger.Information("Logged In");

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
        
        _signals.ApplicationDone.Set();
        
    }
    
}