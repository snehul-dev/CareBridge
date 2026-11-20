using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Admin
{
    public class EntityCountsDto
    {
        public int TotalPatients { get; set; }

        public int TotalDoctors { get; set; }

        public int TotalLabs { get; set; }
    }
}
