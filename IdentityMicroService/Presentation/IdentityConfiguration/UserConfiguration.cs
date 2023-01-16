using IdentityMicroService.Domain.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityMicroService.Presentation.IdentityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        AddInitialData(builder);
    }

    private void AddInitialData(EntityTypeBuilder<Account> builder)
    {
        var user = new Account
        {
            Id = "77619ae5-1ac9-4ee1-84aa-cc32dab2bb68",
            UserName = "username",
            NormalizedUserName = "USERNAME",
            Email = "username@username.com",
            NormalizedEmail = "USERNAME@USERNAME.COM",
            PhoneNumber = "XXXXXXXXXXXXX",
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            SecurityStamp = new Guid().ToString("D")
        };
        user.PasswordHash = PassGenerate(user);
        builder.HasData(user);
    }

    private string PassGenerate(Account user)
    {
        var passHash = new PasswordHasher<Account>();
        return passHash.HashPassword(user, "password");
    }
}