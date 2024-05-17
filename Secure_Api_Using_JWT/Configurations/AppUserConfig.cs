using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Secure_Api_Using_JWT.DbContext;
using Secure_Api_Using_JWT.DbContext.Identity;

namespace Secure_Api_Using_JWT.Configurations
{
    public class AppUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(AU => AU.FirstName).IsRequired();
            builder.Property(AU => AU.lastName).IsRequired();

        }
    }
}
