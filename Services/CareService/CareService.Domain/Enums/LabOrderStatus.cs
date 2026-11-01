using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Domain.Enums
{
    public enum LabOrderStatus
    {

        Pending = 1,
        Assigned = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5
    }
}
