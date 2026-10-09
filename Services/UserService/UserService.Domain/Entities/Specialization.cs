using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Domain.Entities
{
    public class Specialization
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<DoctorProfile> Doctors { get; set; } = new List<DoctorProfile>();

    }
}
