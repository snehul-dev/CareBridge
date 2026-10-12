using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations
{
    public class SpecializationConfiguration
        : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.HasData(
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    Name = "General Medicine"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Name = "Neurology"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Name = "Psychiatry"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    Name = "Obstetrics & Gynecology"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    Name = "Ophthalmology"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    Name = "ENT (Otorhinolaryngology)"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                    Name = "Pulmonology"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000008"),
                    Name = "Gastroenterology"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000009"),
                    Name = "Nephrology"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000010"),
                    Name = "Urology"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000011"),
                    Name = "Endocrinology"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000012"),
                    Name = "General Surgery"
                },
                new Specialization
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000013"),
                    Name = "Dentistry"
                }
            );
        }
    }
} 