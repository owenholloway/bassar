using System.Text;
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

        if (!Directory.Exists("exped")) Directory.CreateDirectory("exped");
        
        foreach (var participants in expedGrouping)
        {
            var fileName = "exped/" + participants.Key.Trim().Replace(".", "");

            var participantData = new StringBuilder();
            var participantMedical = new StringBuilder();

            participantData.Append("Moot ID,NameFirst,NameLast,Contingent,Note No\n");
            participantMedical.Append("Note No,Name,Type,Details\n");

            var noteNo = 1;
            
            foreach (var participant in participants)
            {
                participantData.Append(participant.EventId.ToString("00000"));
                participantData.Append(',');
                participantData.Append(participant.NameFirst);
                participantData.Append(',');
                participantData.Append(participant.NameLast);
                participantData.Append(',');
                participantData.Append(participant.Contingent);     
                participantData.Append(','); 


                foreach (var medicalInformation in participant.MedicalInformation)
                {
                    participantMedical.Append(noteNo.ToString("000"));
                    participantMedical.Append(',');
                    participantMedical.Append(participant.Name.Replace(","," - "));
                    participantMedical.Append(',');
                    participantMedical.Append(medicalInformation.MedicalInformationType.ToString());
                    participantMedical.Append(',');
                    participantMedical.Append(medicalInformation.Name.Replace(",",";"));
                    participantMedical.Append('\n');
                }

                if (participant.MedicalInformation.Any())
                {
                    participantData.Append(noteNo.ToString("000"));
                    noteNo++;
                }
                
                participantData.Append('\n');

            }
            
            File.WriteAllText(fileName + ".participants.csv", participantData.ToString());
            File.WriteAllText(fileName + ".notes.csv", participantMedical.ToString());
            
        }
    }
}