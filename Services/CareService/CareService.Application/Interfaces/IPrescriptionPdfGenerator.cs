using System;
using System.Collections.Generic;
using System.Text;
using CareService.Application.DTOs.Prescriptions;

namespace CareService.Application.Interfaces
{
    public interface IPrescriptionPdfGenerator
    {
        byte[] Generate(PrescriptionPdfDto prescription);
    }
}
