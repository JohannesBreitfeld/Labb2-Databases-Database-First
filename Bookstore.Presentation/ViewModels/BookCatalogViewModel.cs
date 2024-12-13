using Bookstore.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Presentation.ViewModels;

public class BookCatalogViewModel : ViewModelBase
{
	private ObservableCollection<BookDisplay>? _books;

	public ObservableCollection<BookDisplay>? Books
	{
		get => _books; 
		set 
		{ 
			_books = value;
			RaisePropertyChanged();
		}
	}

	public BookCatalogViewModel()
    {
        GetBooks();
    }

	private void GetBooks()
	{
		using var context = new BookstoreContext();

        var books = new ObservableCollection<BookDisplay>
            (
                context.Books
                    .Select
                        (
                            b => new BookDisplay()
                            {
                                Isbn13 = b.Isbn13,
                                Title = b.Title,
                                Language = b.Language,
                                Price = b.Price,
                                DatePublished = b.DatePublished,
                                Authors = new ObservableCollection<AuthorDisplay>(b.Authors
                                    .Select(a => new AuthorDisplay()
                                    {
                                        Id = a.Id,
                                        BirthDate = a.BirthDate,
                                        FirstName = a.FirstName,
                                        LastName = a.LastName
                                    }))
                            }
                        )
            );

        Books = books;
    }

}


