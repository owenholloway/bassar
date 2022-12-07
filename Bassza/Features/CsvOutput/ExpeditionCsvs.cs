using Bassza.Api.Dtos;

namespace Bassza.Features.CsvOutput;

public static class ExpeditionCsvs
{
    public static void GenerateExpeditionCsvs(this OlemsDataModel dataModel)
    {
        var expedGrouping 
            = dataModel
                .Participants
                .Where(pt => !(pt.Status.Contains("Not Proceeding") || pt.Status.Contains("Withdrawn")))
                .GroupBy(pt => pt.Expedition);
        
        
        
        
    }
}