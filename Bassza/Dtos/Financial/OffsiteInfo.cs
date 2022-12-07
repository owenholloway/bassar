using Bassza.Api.Dtos.Participant;

namespace Bassza.Dtos.Financial;

public class OffsiteInfo
{
    public int ParticipantId { get; set; }
    public string ParticipantName { get; set; }
    public string ParticipantEmail{ get; set; }
    
    public OffsiteActivity Activity { get; set; }
    public MedicalInformation MedicalInformation { get; set; }
}