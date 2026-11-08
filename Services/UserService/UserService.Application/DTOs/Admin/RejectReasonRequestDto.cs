using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserService.Application.DTOs.Admin
{
    public class RejectReasonRequestDto
    {
        [Required]
        [StringLength(500 , MinimumLength =5)]
        public string Reason { get; set; } = string.Empty;
    }
}
