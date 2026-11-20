using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTOs.Admin
{
    public class RoleCountsDto
    {
        public int TotalPatients { get; set; }

        public int TotalDoctors { get; set; }

        public int TotalLabs { get; set; }
    }
}
