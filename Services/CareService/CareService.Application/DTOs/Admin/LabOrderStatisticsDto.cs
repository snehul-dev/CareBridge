using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Admin
{
    public class LabOrderStatisticsDto
    {
        public int TotalLabOrders { get; set; }

        public int PendingLabOrders { get; set; }

        public int AssignedLabOrders { get; set; }

        public int CompletedLabOrders { get; set; }

        public int CancelledLabOrders { get; set; }

        public int RejectedLabOrders { get; set; }
    }
}
