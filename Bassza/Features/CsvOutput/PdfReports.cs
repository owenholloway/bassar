using System.Text;
using Bassza.Api.Dtos;
using Serilog;

namespace Bassza.Features.CsvOutput;

public static class PdfReports
{
    public static void PrepareExpeditionPdfReports(this OlemsDataModel dataModel)
    {
        var expedGrouping
           = dataModel
               .Participants
               .Where(pt => !pt.Expedition.Contains("No Expedition"))
               .GroupBy(pt => pt.Expedition);

       if (!Directory.Exists("ExpeditionReports")) Directory.CreateDirectory("ExpeditionReports");
       
       var template = File.ReadAllText("ReportTemplates/Expedition.html");
       var participantTemplate = File.ReadAllText("ReportTemplates/Expedition.Participant.html");
       
       foreach (var participants in expedGrouping)
       {
           Log.Information($"Writing out {participants.Key}");
           var participantData = new StringBuilder();
           var participantMedical = new StringBuilder();
           
           participantMedical.Append("Note No,Name,Type,Details\n");

           var noteNo = 1;
           
           foreach (var participant in participants)
           {
               var model = participantTemplate;

               model = model.Replace("&moot.id&",participant.EventId.ToString("00000"));
               model = model.Replace("&name.first&",participant.NameFirst);
               model = model.Replace("&name.last&",participant.NameLast);
               model = model.Replace("&contingent&", participant.Contingent);


               foreach (var medicalInformation in participant.MedicalInformation)
               {

               }

               if (participant.MedicalInformation.Any())
               {
                   model = model.Replace("&note.no&",noteNo.ToString("000"));
                   noteNo++;
               }
               else
               {
                   
                   model = model.Replace("&note.no&","");
               }

               participantData.Append(model);
               

           }
           
           File.WriteAllText("ExpeditionReports/" + participants.Key + ".html", 
               template.Replace("&participant.info&", participantData.ToString()));

       }
    }
}