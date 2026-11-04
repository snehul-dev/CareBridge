using System;
using System.Collections.Generic;
using System.Text;
using CareBridge.Contracts;
using CareBridge.Contracts.Appointments;

namespace CareService.Application.Interfaces
{
    public interface IAppointmentServiceClient
    {
        Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(Guid appointmentId,
            CancellationToken cancellationToken);
    }
}
