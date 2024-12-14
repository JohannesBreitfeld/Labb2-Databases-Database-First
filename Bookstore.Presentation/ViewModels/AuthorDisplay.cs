using Bookstore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Presentation.ViewModels;

public class AuthorDisplay
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    //public virtual ICollection<Book> Isbn13s { get; set; } = new List<Book>();

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}
