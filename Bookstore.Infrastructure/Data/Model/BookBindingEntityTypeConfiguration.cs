using Bookstore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.Infrastructure.Data.Model;

public class BookBindingEntityTypeConfiguration : IEntityTypeConfiguration<BookBinding>
{
    public void Configure(EntityTypeBuilder<BookBinding> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__BookBind__3214EC07C568C752");

        builder.ToTable("BookBinding");

        builder.HasIndex(e => e.Type, "UQ__BookBind__F9B8A48B4FBC7280").IsUnique();

        builder.Property(e => e.Type).HasMaxLength(20);
    }
}
