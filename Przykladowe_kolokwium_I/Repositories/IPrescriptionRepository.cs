using Przykladowe_kolokwium_I.DTO;

namespace Przykladowe_kolokwium_I.Repositories;

public interface IPrescriptionRepository
{
    Task<IEnumerable<PrescriptionDetailsDto>> GetPrescription(string? doctorLastName); //? - mo¿e byæ nullem

    Task<bool> PatientExists(int patientId);

    Task<bool> DoctorExists(int doctorId);

    Task<PrescriptionDto?> Create(CreatePrescriptionDto dto);
}