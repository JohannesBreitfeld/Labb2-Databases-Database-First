using Bookstore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.Infrastructure.Data.Model;

public class InventoryEntityTypeConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(e => new { e.Isbn13, e.StoreId }).HasName("PK__Inventor__384FB1130CF1DEC9");

        builder.Property(e => e.Isbn13)
            .HasMaxLength(13)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("ISBN13");

        builder.HasOne(d => d.Isbn13Navigation).WithMany(p => p.Inventories)
            .HasForeignKey(d => d.Isbn13)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK__Inventori__ISBN1__5CD6CB2B");

        builder.HasOne(d => d.Store).WithMany(p => p.Inventories)
            .HasForeignKey(d => d.StoreId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK__Inventori__Store__5DCAEF64");
    }
}
