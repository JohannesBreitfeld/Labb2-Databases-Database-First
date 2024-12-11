using Bookstore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Bookstore.Infrastructure.Data.Model;

public partial class BookstoreContext : DbContext
{
    public BookstoreContext()
    {
    }

    public BookstoreContext(DbContextOptions<BookstoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookBinding> BookBindings { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config =new ConfigurationBuilder().AddUserSecrets<BookstoreContext>().Build();
        var connectionString = config["ConnectionString"];
        optionsBuilder.UseSqlServer(connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new AuthorEntityTypeConfiguration().Configure(modelBuilder.Entity<Author>());
        new BookBindingEntityTypeConfiguration().Configure(modelBuilder.Entity<BookBinding>());
        new BookEntityTypeConfiguration().Configure(modelBuilder.Entity<Book>());
        new InventoryEntityTypeConfiguration().Configure(modelBuilder.Entity<Inventory>());
        new LanguageEntityTypeConfiguration().Configure(modelBuilder.Entity<Language>());
        new StoreEntityTypeConfiguration().Configure(modelBuilder.Entity<Store>());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
