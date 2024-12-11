using Bookstore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.Infrastructure.Data.Model;

public class AuthorEntityTypeConfiguration : IEntityTypeConfiguration<Author>
{


    public void Configure(EntityTypeBuilder<Author> builder)
    {
    
            builder.HasKey(e => e.Id).HasName("PK__Authors__3214EC071124C126");

            builder.Property(e => e.FirstName).HasMaxLength(50);
            builder.Property(e => e.LastName).HasMaxLength(50);
        
    }
}
