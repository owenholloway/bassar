using System.Text;
using Bassza.Api.Dtos;
using Bassza.Api.Dtos.Participant;
using Bassza.ReportTemplates;
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

       if (!Directory.Exists("ExpeditionHtmlReports")) Directory.CreateDirectory("ExpeditionHtmlReports");
       if (!Directory.Exists("ExpeditionPdfReports")) Directory.CreateDirectory("ExpeditionPdfReports");
       
       foreach (var participants in expedGrouping)
       {      
           var template = File.ReadAllText("ReportTemplates/Expedition.html");
           var participantTemplate = File.ReadAllText("ReportTemplates/Expedition.Participant.html");
           var noteTemplate = File.ReadAllText("ReportTemplates/Expedition.Note.html");
           Log.Information($"Writing out {participants.Key}");
           var participantData = new StringBuilder();
           var participantNote = new StringBuilder();
           
           var noteNo = 1;

           var sortedParticipants 
               = participants
               .OrderBy(pt => pt.Name);
           
           foreach (var participant in sortedParticipants)
           {
               var model = participantTemplate;

               model = model.Replace("&moot.id&",participant.EventId.ToString("00000"));
               model = model.Replace("&name.first&",participant.NameFirst);
               model = model.Replace("&name.last&",participant.NameLast);
               model = model.Replace("&contingent&", participant.Contingent);


               foreach (var medicalInformation in participant.MedicalInformation)
               {
                   var note = noteTemplate;

                   note = note.Replace("&note.no&", noteNo.ToString("000"));
                   note = note.Replace("&note.type&", medicalInformation.MedicalInformationType.ToString());
                   var detail = new StringBuilder();
                   
                   if (medicalInformation.MedicalInformationType == MedicalInformationType.MedicalCondition)
                   {
                       detail.Append(medicalInformation.Name);
                       detail.Append("<br>");
                       detail.Append(medicalInformation.FurtherInformation);
                   }
                   
                   if (medicalInformation.MedicalInformationType == MedicalInformationType.Medication)
                   {
                       detail.Append(medicalInformation.Name);
                       detail.Append("<br>");
                       detail.Append(medicalInformation.Dosage);
                       detail.Append("<br>");
                       detail.Append(medicalInformation.MethodOfAdministration);
                   }
                   
                   if (medicalInformation.MedicalInformationType == MedicalInformationType.MedicalAid)
                   {
                       detail.Append(medicalInformation.Name);
                       detail.Append("<br>");
                       detail.Append(medicalInformation.Reason);
                   }
                   
                   if (medicalInformation.MedicalInformationType == MedicalInformationType.Allergies)
                   {
                       detail.Append(medicalInformation.Name);
                       detail.Append("<br>");
                       detail.Append(medicalInformation.Reaction);
                       detail.Append("<br>");
                       detail.Append(medicalInformation.Treatment);
                   }
                   
                   if (medicalInformation.MedicalInformationType == MedicalInformationType.DietaryRequirements)
                   {
                       detail.Append(medicalInformation.Name);
                       detail.Append("<br>");
                       detail.Append(medicalInformation.DietCode);
                       detail.Append("<br>");
                       detail.Append(medicalInformation.Information);
                   }

                   note = note.Replace("&note.details&", detail.ToString());

                   participantNote.Append(note);

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

           template = template.Replace("&expedition.name&", participants.Key);
           template = template.Replace("&participant.info&", participantData.ToString());
           template = template.Replace("&note.info&", participantNote.ToString());

           var htmlFilePath = "./ExpeditionHtmlReports/" 
                              + participants.Key.Trim()
                                  .Replace(".", "")
                                  .Replace(" ","") + ".html";
           
           var pdfFilePath = "./ExpeditionPdfReports/" 
                              + participants.Key.Trim()
                                  .Replace(".", "")
                                  .Replace(" ","") + ".pdf";
           
           File.WriteAllText(htmlFilePath, template);
           
           WkhtmltoPdfRunner.Run("wkhtmltopdf", $"{htmlFilePath} {pdfFilePath}");
           
           

       }
    }
}