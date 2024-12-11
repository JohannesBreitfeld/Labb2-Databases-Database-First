using Bookstore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.Infrastructure.Data.Model;

public class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(e => e.Isbn13).HasName("PK__Books__3BF79E036357BB6A");

        builder.Property(e => e.Isbn13)
            .HasMaxLength(13)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("ISBN13");
        builder.Property(e => e.Price).HasColumnType("smallmoney");
        builder.Property(e => e.Title).HasMaxLength(100);

        builder.HasOne(d => d.Binding).WithMany(p => p.Books)
            .HasForeignKey(d => d.BindingId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Books__BindingId__5441852A");

        builder.HasOne(d => d.Language).WithMany(p => p.Books)
            .HasForeignKey(d => d.LanguageId)
            .HasConstraintName("FK__Books__LanguageI__52593CB8");

        builder.HasMany(d => d.Authors).WithMany(p => p.Isbn13s)
            .UsingEntity<Dictionary<string, object>>(
                "AuthorsBook",
                r => r.HasOne<Author>().WithMany()
                    .HasForeignKey("AuthorId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AuthorsBo__Autho__5812160E"),
                l => l.HasOne<Book>().WithMany()
                    .HasForeignKey("Isbn13")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AuthorsBo__ISBN1__571DF1D5"),
                j =>
                {
                    j.HasKey("Isbn13", "AuthorId").HasName("PK__AuthorsB__6CFA31C2A733D37D");
                    j.ToTable("AuthorsBooks");
                    j.IndexerProperty<string>("Isbn13")
                        .HasMaxLength(13)
                        .IsUnicode(false)
                        .IsFixedLength()
                        .HasColumnName("ISBN13");
                    j.IndexerProperty<int>("AuthorId").HasColumnName("AuthorID");
                });
    }
}
