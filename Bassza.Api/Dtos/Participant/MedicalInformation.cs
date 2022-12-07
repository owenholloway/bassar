namespace Bassza.Api.Dtos.Participant;

public class MedicalInformation
{
    public MedicalInformationType MedicalInformationType { get; set; }
    
    public string Name { get; set; } = "Undefined";
    
    // Medical Conditions
    public string? FurtherInformation { get; set; } = string.Empty;
    
    // Medications
    public string? Dosage { get; set; } = string.Empty;
    public string? MethodOfAdministration { get; set; } = string.Empty;
    
    // Medical Aids
    public string? Reason { get; set; } = string.Empty;
    
    // Allergies
    public string? Reaction { get; set; } = string.Empty;
    public string? Treatment { get; set; } = string.Empty;
    
    // Dietary
    public string? DietCode { get; set; } = string.Empty;
    public string? Information { get; set; } = string.Empty;


}