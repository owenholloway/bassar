namespace Bassza.Api.Dtos.Participant;

public class MedicalInformation
{
    public MedicalInformationType MedicalInformationType { get; set; }
    
    public string Name { get; set; } = "Undefined";
    
    // Medical Conditions
    public string? FurtherInformation = "";
    
    // Medications
    public string? Dosage { get; set; }
    public string? MethodOfAdministration { get; set; }
    
    // Medical Aids
    public string? Reason { get; set; }
    
    // Allergies
    public string? Reaction { get; set; }
    public string? Treatment { get; set; }
    
    // Dietary
    public string? DietCode { get; set; }
    public string? Information { get; set; }


}