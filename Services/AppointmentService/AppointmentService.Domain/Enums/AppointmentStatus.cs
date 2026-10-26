using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Domain.Enums
{
    public enum AppointmentStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3,
        Completed = 4,
        Rescheduled = 5,
        Rejected = 6
    }
}
