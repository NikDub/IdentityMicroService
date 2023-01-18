using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityMicroService.Presentation.IdentityConfiguration;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        AddInitialData(builder);
    }

    private void AddInitialData(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData
        (
            new IdentityUserRole<string>
            {
                RoleId = "8d734af4-145b-4778-b320-14effafc96b3",
                UserId = "77619ae5-1ac9-4ee1-84aa-cc32dab2bb68"
            }
        );
    }
}