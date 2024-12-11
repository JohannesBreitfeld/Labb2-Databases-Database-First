namespace Bookstore.Domain;

public partial class Author
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public virtual ICollection<Book> Isbn13s { get; set; } = new List<Book>();
}
