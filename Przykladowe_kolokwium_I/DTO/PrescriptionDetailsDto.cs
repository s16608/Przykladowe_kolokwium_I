namespace Przykladowe_kolokwium_I.DTO;

public class PrescriptionDetailsDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public string PatientLastName { get; set; } = null!;
    public string DoctorLastName { get; set; } = null!;
}