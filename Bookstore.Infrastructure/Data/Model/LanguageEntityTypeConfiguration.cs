using Bookstore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.Infrastructure.Data.Model;

public class LanguageEntityTypeConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Language__3214EC0765749BE2");

        builder.HasIndex(e => e.Name, "UQ__Language__737584F667DF1F3F").IsUnique();

        builder.Property(e => e.Name).HasMaxLength(50);
    }
}
