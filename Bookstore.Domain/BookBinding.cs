namespace Bookstore.Domain;

public partial class BookBinding
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
