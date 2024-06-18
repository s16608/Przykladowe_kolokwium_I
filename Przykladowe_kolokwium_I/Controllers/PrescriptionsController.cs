using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Przykladowe_kolokwium_I.DTO;
using Przykladowe_kolokwium_I.Repositories;

namespace Przykladowe_kolokwium_I.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public PrescriptionsController(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PrescriptionDetailsDto>>> Browse(string? doctorLastName)
        => Ok(await _prescriptionRepository.GetPrescription(doctorLastName));

    [HttpPost]
    public async Task<ActionResult<PrescriptionDto>> Create(CreatePrescriptionDto dto)
    {
        if (dto.DueDate <= dto.Date)
            return BadRequest("Niepoprawna data!");

        var doctorExists = await _prescriptionRepository.DoctorExists(dto.IdDoctor);

        if (doctorExists is false)
            return BadRequest("Doktor nie istnieje w bazie!");

        var patientExists = await _prescriptionRepository.PatientExists(dto.IdPatient);

        if (patientExists is false)
            return BadRequest("Pacjent nie istnieje w bazie!");

        var createdDto = await _prescriptionRepository.Create(dto);

        if (createdDto is null)
            return BadRequest("Tworzenie obiektu nie powiod³o siê, dane s¹ nieprawid³owe");

        return createdDto;
    }
}