using System.Data.SqlClient;
using Przykladowe_kolokwium_I.DTO;

namespace Przykladowe_kolokwium_I.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly string _connectionString;

    public PrescriptionRepository(IConfiguration configuration)
    {
        //?? - je¿eli connection string z konfiguracji jest null to throwany jest Exception
        _connectionString = configuration["DefaultConnection"] ?? throw new ArgumentException("Brak podanego connection stringa do bazy!");
    }

    public async Task<PrescriptionDto?> Create(CreatePrescriptionDto dto)
    {
        using var sqlConnection = new SqlConnection(_connectionString);

        await sqlConnection.OpenAsync();

        var insertSqlCommand = new SqlCommand
        {
            CommandText = "INSERT INTO Prescription VALUES(@Date, @DueDate, @IdPatient, @IdDoctor); SELECT SCOPE_IDENTITY()",
            Connection = sqlConnection
        };

        insertSqlCommand.Parameters.AddWithValue("@Date", dto.Date);
        insertSqlCommand.Parameters.AddWithValue("@DueDate", dto.DueDate);
        insertSqlCommand.Parameters.AddWithValue("@IdPatient", dto.IdPatient);
        insertSqlCommand.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);

        var result = await insertSqlCommand.ExecuteScalarAsync();

        int id = Convert.ToInt32((decimal)result);

        //int id = result is decimal dec ? Convert.ToInt32(dec) : 0; //konwertujemy na decimal, aby potem zamieniæ na inta. Je¿eli nie jest decimalem to przyjmujemy 0, powinniœmy w sumie zwracaæ Exception

        var selectSqlCommand = new SqlCommand
        {
            CommandText = "SELECT IdPrescription, Date, DueDate, IdPatient, IdDoctor FROM Prescription WHERE IdPrescription = @Id",
            Connection = sqlConnection
        };

        selectSqlCommand.Parameters.AddWithValue("@Id", id);

        var reader = await selectSqlCommand.ExecuteReaderAsync();

        PrescriptionDto? prescriptionDto = null;

        while (reader.Read())
        {
            prescriptionDto = new PrescriptionDto
            {
                IdPrescription = int.Parse(reader["IdPrescription"].ToString()!),
                DueDate = DateTime.Parse(reader["DueDate"].ToString()!),
                Date = DateTime.Parse(reader["Date"].ToString()!),
                IdPatient = int.Parse(reader["IdPatient"].ToString()!),
                IdDoctor = int.Parse(reader["IdDoctor"].ToString()!)
            };
        }

        return prescriptionDto;
    }

    public async Task<bool> DoctorExists(int doctorId)
    {
        using var sqlConnection = new SqlConnection(_connectionString);

        await sqlConnection.OpenAsync();

        var selectCom = new SqlCommand()
        {
            Connection = sqlConnection,
            CommandText = "SELECT COUNT(1) FROM Doctor WHERE IdDoctor = @IdDoctor"
        };

        selectCom.Parameters.AddWithValue("@IdDoctor", doctorId);

        var result = await selectCom.ExecuteScalarAsync();

        if (result is null)
            return false;

        return (int)result > 0;
    }

    public async Task<bool> PatientExists(int patientId)
    {
        using var sqlConnection = new SqlConnection(_connectionString);

        await sqlConnection.OpenAsync();

        var selectCom = new SqlCommand()
        {
            Connection = sqlConnection,
            CommandText = "SELECT COUNT(1) FROM Patient WHERE IdPatient = @IdPatient"
        };

        selectCom.Parameters.AddWithValue("@IdPatient", patientId);

        var result = await selectCom.ExecuteScalarAsync();

        if (result is null)
            return false;

        return (int)result > 0;
    }

    public async Task<IEnumerable<PrescriptionDetailsDto>> GetPrescription(string? doctorLastName)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = string.Empty;

        //Je¿eli dwie kolumny s¹ takie same czyli LastName to musimy jedn¹ z ni¹ nazwaæ jakoœ customowo, w tym przypadku nazywamy j¹ DoctorLastName zamiast LastName
        if (string.IsNullOrEmpty(doctorLastName) is false)
            query =
                "SELECT IdPrescription, Date, DueDate, Patient.LastName, Doctor.LastName as 'DoctorLastName' FROM Prescription INNER JOIN Doctor ON Prescription.IdDoctor = Doctor.IdDoctor INNER JOIN Patient ON Patient.IdPatient = Prescription.IdPatient " +
                    "WHERE Doctor.LastName = @DoctorLastName";
        else
            query = "SELECT IdPrescription, Date, DueDate, Patient.LastName, Doctor.LastName as 'DoctorLastName' FROM Prescription INNER JOIN Doctor ON Prescription.IdDoctor = Doctor.IdDoctor INNER JOIN Patient ON Patient.IdPatient = Prescription.IdPatient";

        await using var command = new SqlCommand(query, connection);

        if (string.IsNullOrEmpty(doctorLastName) is false)
            command.Parameters.AddWithValue("@DoctorLastName", doctorLastName);

        var reader = await command.ExecuteReaderAsync();

        List<PrescriptionDetailsDto> prescriptionDtos = [];

        while (reader.Read())
        {
            var dto = new PrescriptionDetailsDto
            {
                IdPrescription = int.Parse(reader["IdPrescription"].ToString()!),
                DueDate = DateTime.Parse(reader["DueDate"].ToString()!),
                Date = DateTime.Parse(reader["Date"].ToString()!),
                PatientLastName = reader["LastName"].ToString()!,
                DoctorLastName = reader["DoctorLastName"].ToString()!
            };

            prescriptionDtos.Add(dto);
        }

        return prescriptionDtos;
    }
}