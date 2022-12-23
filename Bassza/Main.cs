using System.Security.Authentication;
using Bassza.Api.Dtos;
using Bassza.Api.Dtos.Participant;
using Bassza.Api.Features;
using Bassza.Api.Features.Processors;
using Bassza.Features;
using Bassza.Features.CsvOutput;
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
        
        var loggedIn = false;
        
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
        // await Task.Delay(200);
        // await dataModel.ProcessPaymentsExtended(saveDataForTest: true, consumeTestData: consumeTestData);
        await Task.Delay(200);
        await dataModel.ProcessPayment(saveDataForTest: true, consumeTestData: consumeTestData);
        await Task.Delay(200);
        await dataModel.ProcessMedicalReportResponse(saveDataForTest: true, consumeTestData: consumeTestData);
        await Task.Delay(200);
        await dataModel.ProcessOffsiteActivies(saveDataForTest: true, consumeTestData: consumeTestData);
        await Task.Delay(200);
        await dataModel.ProcessTransportDetails(saveDataForTest: true, consumeTestData: consumeTestData);
        await Task.Delay(200);
        await dataModel.ProcessOffsiteExpeditions(saveDataForTest: true, consumeTestData: consumeTestData);
        
        var model = dataModel.CalculatePosition();
        var offsiteFlagged = dataModel.Participants.Where(pt => pt.OffsiteDiscrepancy);
        var fullDietary = dataModel.ProcessDietaries();
        var offSiteDietaryReport = dataModel.ProcessOffsiteDietaries();

        var allPaymentsGrouped = dataModel
            .Participants
            .Select(pt => pt.FinancialPosition)
            .SelectMany(fp => fp.Payments);

        var paymentsGrouped = allPaymentsGrouped as Payment[] ?? allPaymentsGrouped.ToArray();
        var debitedPayments = paymentsGrouped
            .Where(pt => pt.ReceivedValue != null && pt.ReceivedDate != null)
            .ToList();

        //This is disable in container as it has nowhere to store the reports
        //dataModel.PrepareExpeditionPdfReports();

        var updateTasks = new List<Task>();
        
        updateTasks.Add(_sheetsApiManager.UpdateTravelDetails(dataModel));
        updateTasks.Add(_sheetsApiManager.UpdateFinancialPosition(model));
        updateTasks.Add(_sheetsApiManager.UpdateDebitedPayments(debitedPayments));
        updateTasks.Add(_sheetsApiManager.UpdateDataModel(dataModel));
        updateTasks.Add(_sheetsApiManager.UpdateDietariesSheet(fullDietary));
        updateTasks.Add(_sheetsApiManager.UpdateOffsiteDietariesSheet(offSiteDietaryReport));
        updateTasks.Add(_sheetsApiManager.UpdateOffsiteFullDaySheet(dataModel));
        updateTasks.Add(_sheetsApiManager.UpdateOffsiteEmails(dataModel));
        updateTasks.Add(_sheetsApiManager.UpdateOffsiteTourDietariesSheet(offSiteDietaryReport));

        //updateTasks.Add(_sheetsApiManager.UpdateLiabilityPayments(paymentsGrouped.ToList()));
        
        foreach (var updateTask in updateTasks)
        {
            await updateTask.WaitAsync(new CancellationToken());
        }

    }
    
}