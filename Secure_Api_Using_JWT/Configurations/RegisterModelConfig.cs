using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Secure_Api_Using_JWT.Models;

namespace Secure_Api_Using_JWT.Configurations
{
    public class RegisterModelConfig : IEntityTypeConfiguration<RegisterModel>
    {
        public void Configure(EntityTypeBuilder<RegisterModel> builder)
        {
            builder.Property(R => R.FirstName).IsRequired();
            builder.Property(R => R.LastName).IsRequired();
            builder.Property(R => R.UserName).IsRequired();
            builder.Property(R => R.Email).IsRequired();
            builder.Property(R => R.Password).IsRequired();
        }
    }
}
