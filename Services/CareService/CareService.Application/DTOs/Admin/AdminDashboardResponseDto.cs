using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Admin
{
    public class AdminDashboardResponseDto
    {
        public int TotalConsultations { get; set; }
        public int CompletedConsultations { get; set; }
        public int ScheduledConsultations { get; set; }
        public int InProgressConsultations { get; set; }
        public int CancelledConsultations { get; set; }

        public int TotalLabOrders { get; set; }
        public int PendingLabOrders { get; set; }
        public int CompletedLabOrders { get; set; }
    }
}
