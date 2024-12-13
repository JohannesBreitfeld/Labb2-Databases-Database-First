using Bookstore.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Presentation.ViewModels;

public class BookDisplay : ViewModelBase
{
    private string? _authorsString;

    public string Isbn13 { get; set; } = null!;

    public string Title { get; set; } = null!;

    public decimal Price { get; set; }

    public DateOnly? DatePublished { get; set; }

    public  BookBinding Binding { get; set; } = null!;

    public  Language? Language { get; set; }

    public string? AuthorsString
    {
        get
        {
            string authors = string.Empty;

            foreach (var author in Authors)
            {
                if (Authors.Count == 1 || author == Authors.LastOrDefault())
                {
                    authors += $"{author.FirstName} {author.LastName}";
                }
                else
                {
                    authors += $"{author.FirstName} {author.LastName}, ";
                }
            }

            AuthorsString = authors;
            return authors;
        }
        set
        {
            if( _authorsString != value)
            {
                _authorsString = value;
                RaisePropertyChanged();
            }
        }
    }

    public  ObservableCollection<AuthorDisplay> Authors { get; set; } = new ObservableCollection<AuthorDisplay>();
}
