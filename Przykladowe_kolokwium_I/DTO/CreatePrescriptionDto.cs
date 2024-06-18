namespace Przykladowe_kolokwium_I.DTO
{
    public sealed class CreatePrescriptionDto
    {
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int IdPatient { get; set; }
        public int IdDoctor { get; set; }
    }
}