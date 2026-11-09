using System;
using System.Collections.Generic;
using System.Text;

namespace CareBridge.Contracts.Users
{
    public class PrescriptionProfileResponseDto
    {
        public DoctorPrescriptionProfileDto Doctor { get; set; } = null!;

        public PatientPrescriptionProfileDto Patient { get; set; } = null!;
    }
}
