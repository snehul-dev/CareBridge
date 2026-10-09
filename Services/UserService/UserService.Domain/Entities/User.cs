using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UserService.Domain.Enums;

namespace UserService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber {  get; set; } = string.Empty;
        public string PasswordHash {  get; set; } = string.Empty;
        public Gender? Gender {  get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public PatientProfile? PatientProfile { get; set; } 
        public DoctorProfile? DoctorProfile { get; set; } 
        public LabProfile? LabProfile { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();


    }
}
