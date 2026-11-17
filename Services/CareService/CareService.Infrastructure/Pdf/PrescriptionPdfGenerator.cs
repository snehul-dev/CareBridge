using CareService.Application.DTOs.Prescriptions;
using CareService.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CareService.Infrastructure.Pdf
{
    public class PrescriptionPdfGenerator : IPrescriptionPdfGenerator
    {
        static PrescriptionPdfGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] Generate(PrescriptionPdfDto prescription)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("CAREBRIDGE")
                            .FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                        column.Item().Text("Medical Prescription")
                            .FontSize(16).Bold();
                        column.Item().PaddingTop(5).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Spacing(8);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Prescription ID: {prescription.PrescriptionId}");
                            row.RelativeItem().AlignRight()
                                .Text($"Date: {prescription.CreatedAt:dd MMM yyyy}");
                        });

                        column.Item().Text("DOCTOR DETAILS").Bold();
                        column.Item().Text($"Name: {prescription.DoctorName}");
                        if (!string.IsNullOrWhiteSpace(prescription.Qualification))
                            column.Item().Text($"Qualification: {prescription.Qualification}");
                        if (!string.IsNullOrWhiteSpace(prescription.Specialization))
                            column.Item().Text($"Specialization: {prescription.Specialization}");
                        if (!string.IsNullOrWhiteSpace(prescription.LicenseNumber))
                            column.Item().Text($"License No: {prescription.LicenseNumber}");
                        if (!string.IsNullOrWhiteSpace(prescription.ClinicName))
                            column.Item().Text($"Clinic: {prescription.ClinicName}");
                        if (!string.IsNullOrWhiteSpace(prescription.DoctorAddress))
                            column.Item().Text($"Address: {prescription.DoctorAddress}");

                        column.Item().PaddingTop(5).LineHorizontal(1);

                        column.Item().Text("PATIENT DETAILS").Bold();
                        column.Item().Text($"Name: {prescription.PatientName}");
                        if (!string.IsNullOrWhiteSpace(prescription.PatientEmail))
                            column.Item().Text($"Email: {prescription.PatientEmail}");
                        if (!string.IsNullOrWhiteSpace(prescription.PatientPhone))
                            column.Item().Text($"Phone: {prescription.PatientPhone}");
                        if (!string.IsNullOrWhiteSpace(prescription.BloodGroup))
                            column.Item().Text($"Blood Group: {prescription.BloodGroup}");

                        column.Item().PaddingTop(5).LineHorizontal(1);

                        column.Item().Text("PRESCRIBED MEDICINES").Bold().FontSize(14);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Medicine").Bold();
                                header.Cell().Element(CellStyle).Text("Dosage").Bold();
                                header.Cell().Element(CellStyle).Text("Frequency").Bold();
                                header.Cell().Element(CellStyle).Text("Duration").Bold();
                                header.Cell().Element(CellStyle).Text("Qty").Bold();
                                header.Cell().Element(CellStyle).Text("Instructions").Bold();
                            });

                            foreach (var medicine in prescription.Items)
                            {
                                var name = medicine.MedicineName;
                                if (!string.IsNullOrWhiteSpace(medicine.GenericName))
                                    name += $" ({medicine.GenericName})";
                                if (!string.IsNullOrWhiteSpace(medicine.DosageForm))
                                    name += $" - {medicine.DosageForm}";

                                table.Cell().Element(CellStyle).Text(name);
                                table.Cell().Element(CellStyle).Text(medicine.Dosage);
                                table.Cell().Element(CellStyle).Text(medicine.Frequency);
                                table.Cell().Element(CellStyle).Text(medicine.Duration);
                                table.Cell().Element(CellStyle)
                                    .Text(medicine.Quantity?.ToString() ?? "-");
                                table.Cell().Element(CellStyle)
                                    .Text(medicine.Instructions ?? string.Empty);
                            }
                        });

                        if (!string.IsNullOrWhiteSpace(prescription.Notes))
                        {
                            column.Item().PaddingTop(10).Text("NOTES").Bold();
                            column.Item().Text(prescription.Notes);
                        }
                    });

                    page.Footer().AlignCenter()
                        .Text("Generated by CareBridge").FontSize(9);
                });
            }).GeneratePdf();
        }

        private static IContainer CellStyle(IContainer container)
            => container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
    }
}
