namespace CareBridge.Contracts.Users;

public class UserPrescriptionProfileDto
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    public string? BloodGroup { get; set; }

    public string? Qualification { get; set; }
    public string? HospitalOrClinicName { get; set; }
    public string? DoctorAddress { get; set; }
    public string? City { get; set; }
    public string? SpecializationName { get; set; }
}
