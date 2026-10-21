using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Domain.Enums
{
    public enum ReferralStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3,
        Completed = 4,
        Cancelled = 5
    }
}
