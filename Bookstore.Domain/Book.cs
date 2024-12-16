namespace Bookstore.Domain;

public partial class Book
{
    public string Isbn13 { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int? LanguageId { get; set; }

    public decimal Price { get; set; }

    public DateOnly? DatePublished { get; set; }

    public int BindingId { get; set; }

    public virtual BookBinding Binding { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual Language? Language { get; set; }

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public override string ToString()
    {
        return Title;
    }
}
