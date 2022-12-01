using IdentityMicroService.Domain.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityMicroService.Presentation.IdentityConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            AddInitialData(builder);
        }

        private void AddInitialData(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData
            (
                new IdentityRole
                {
                    Id = "8d734af4-145b-4778-b320-14effafc96b3",
                    Name = nameof(UserRole.Receptionist),
                    NormalizedName = nameof(UserRole.Receptionist).ToUpper()
                },
                new IdentityRole
                {
                    Name = nameof(UserRole.Doctor),
                    NormalizedName = nameof(UserRole.Doctor).ToUpper()
                },
                new IdentityRole
                {
                    Name = nameof(UserRole.Patient),
                    NormalizedName = nameof(UserRole.Patient).ToUpper()
                }
            );
        }
    }
}
