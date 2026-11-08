using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Domain.Enums
{
    public enum LabOrderStatus
    {

        AwaitingLabSelection = 1,
        PendingAcceptance = 2,
        Accepted = 3,
        Rejected = 4,
        SampleCollected = 5,
        InProgress = 6,
        Completed = 7,
        Cancelled = 8
    }
}

