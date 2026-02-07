using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Selu383.SP26.Api.Features.Logins;

public class LoginConfiguration : IEntityTypeConfiguration<Login>
{
    public void Configure(EntityTypeBuilder<Login> builder)
    {
        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(120);
        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(120);
    }
}