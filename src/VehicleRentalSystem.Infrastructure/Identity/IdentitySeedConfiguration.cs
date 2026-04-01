using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VehicleRentalSystem.Infrastructure.Identity;

public class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>
{
    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        var adminUser = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin@example.com",
            NormalizedUserName = "ADMIN@EXAMPLE.COM",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        adminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(adminUser, "P@ssw0rd!");

        builder.HasData(adminUser);
    }
}

public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "Courier", NormalizedName = "COURIER" },
            new IdentityRole { Id = "3", Name = "Motorcycle", NormalizedName = "MOTORCYCLE" },
            new IdentityRole { Id = "4", Name = "Rental", NormalizedName = "RENTAL" }
        );
    }
}

public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(
            new IdentityUserRole<string>
            {
                UserId = "1",
                RoleId = "1"
            }
        );
    }
}

public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        builder.HasData(
            new IdentityUserClaim<string> { Id = 1, UserId = "1", ClaimType = "Courier", ClaimValue = "Get" },
            new IdentityUserClaim<string> { Id = 2, UserId = "1", ClaimType = "Courier", ClaimValue = "Add" },
            new IdentityUserClaim<string> { Id = 3, UserId = "1", ClaimType = "Courier", ClaimValue = "Update" },
            new IdentityUserClaim<string> { Id = 4, UserId = "1", ClaimType = "Courier", ClaimValue = "Delete" },
            new IdentityUserClaim<string> { Id = 5, UserId = "1", ClaimType = "Motorcycle", ClaimValue = "Get" },
            new IdentityUserClaim<string> { Id = 6, UserId = "1", ClaimType = "Motorcycle", ClaimValue = "Add" },
            new IdentityUserClaim<string> { Id = 7, UserId = "1", ClaimType = "Motorcycle", ClaimValue = "Update" },
            new IdentityUserClaim<string> { Id = 8, UserId = "1", ClaimType = "Motorcycle", ClaimValue = "Delete" },
            new IdentityUserClaim<string> { Id = 9, UserId = "1", ClaimType = "Rental", ClaimValue = "Get" },
            new IdentityUserClaim<string> { Id = 10, UserId = "1", ClaimType = "Rental", ClaimValue = "Add" },
            new IdentityUserClaim<string> { Id = 11, UserId = "1", ClaimType = "Rental", ClaimValue = "Update" },
            new IdentityUserClaim<string> { Id = 12, UserId = "1", ClaimType = "Rental", ClaimValue = "Delete" }
        );
    }
}
