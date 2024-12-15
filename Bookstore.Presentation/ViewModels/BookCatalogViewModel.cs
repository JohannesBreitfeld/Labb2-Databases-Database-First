using Bookstore.Domain;
using Bookstore.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Bookstore.Presentation.ViewModels;

public class BookCatalogViewModel : ViewModelBase
{
	private ObservableCollection<BookDisplay>? _books;
    private BookDisplay? _selectedBook;
    private AuthorDisplay? _authorToAdd;
    private ObservableCollection<Language?>? _languages;

    private ObservableCollection<AuthorDisplay>? _addableAuthors;

    private string _selectedBookIsbn = null!;

    public string SelectedBookIsbn
    {
        get => _selectedBookIsbn;
        set
        {
             _selectedBookIsbn = value;
             RaisePropertyChanged();
            if (IsValidISBN13(value) && SelectedBook != null)
            {
                SelectedBook!.Isbn13 = _selectedBookIsbn;
            }
        }
    }

    private string _isbnMessege = string.Empty;
    public string IsbnMessege 
    {
        get => _isbnMessege;
        set
        {
            _isbnMessege = value;
            RaisePropertyChanged();
        }
    }
    public ObservableCollection<AuthorDisplay>? AddableAuthors
    {
        get => _addableAuthors; 
        set 
        { 
            _addableAuthors = value;
            RaisePropertyChanged();
        }
    }
    public ObservableCollection<Language?>? Languages 
    {
        get => _languages;
        set
        {
            _languages = value;
            RaisePropertyChanged();
        }
    }
    public BookDisplay? SelectedBook
    {
        get => _selectedBook; 
        set
        {
            _selectedBook = value;
            SelectedBook!.SelectedAuthor = SelectedBook.Authors.FirstOrDefault();

            RaisePropertyChanged();
            RemoveAuthorCommand.RaiseCanExecuteChanged();
            GetAddableAuthors();
            if(SelectedBook.Isbn13 != null)
            {
                SelectedBookIsbn = SelectedBook.Isbn13;
            }
            if (SelectedBook.Language != null)
            {
                SelectedBook.Language = Languages?.FirstOrDefault(l => l.Id == SelectedBook.Language.Id);
            }
            if (SelectedBook.Binding != null)
            {
                SelectedBook.Binding = Bindings.FirstOrDefault(b => b.Id == SelectedBook.Binding.Id);
            }
        }
    }
    public ObservableCollection<BookDisplay>? Books
	{
		get => _books; 
		set 
		{ 
			_books = value;
			RaisePropertyChanged();
		}
	}
    public AuthorDisplay? AuthorToAdd
    {
        get => _authorToAdd;
        set
        {
            _authorToAdd = value;
            RaisePropertyChanged();
            AddAuthorCommand.RaiseCanExecuteChanged();
        }
    }
    public ObservableCollection<BookBinding> Bindings { get; set; }


    public BookCatalogViewModel()
    {
        GetBooks();     
        RemoveAuthorCommand = new DelegateCommand(RemoveAuthorFromBook, (object? _) => SelectedBook?.SelectedAuthor != null);
        AddAuthorCommand = new DelegateCommand(AddAuthorToBook, (object? _) => AuthorToAdd != null);
    }

    private void RemoveAuthorFromBook(object obj)
    {
        if (SelectedBook != null && SelectedBook.SelectedAuthor != null)
        {
            SelectedBook.Authors.Remove(SelectedBook.SelectedAuthor);
            SelectedBook.RaisePropertyChanged(nameof(SelectedBook.AuthorsString));
        }
    }

    private void AddAuthorToBook(object obj)
    {
        if (SelectedBook != null && AuthorToAdd != null)
        {
            SelectedBook.Authors.Add(AuthorToAdd);
            SelectedBook.RaisePropertyChanged(nameof(SelectedBook.AuthorsString));
            GetAddableAuthors();
        }
    }

    public DelegateCommand AddAuthorCommand { get; }
    public DelegateCommand RemoveAuthorCommand { get; }
    
    private void GetLanguages()
    {
        using (var context = new BookstoreContext())
        {
            Languages = new ObservableCollection<Language>(context.Languages.Distinct().ToList());
        }
    }
    private void GetBindings()
    {
        using (var context = new BookstoreContext())
        {
            Bindings = new ObservableCollection<BookBinding>(context.BookBindings.Distinct().ToList());
        }
    }

	private void GetBooks()
	{
        GetLanguages();
        GetBindings();
        using var context = new BookstoreContext();

        var books = new ObservableCollection<BookDisplay>
            (
                context.Books
                .Select(b => new BookDisplay()
                        {
                            Isbn13 = b.Isbn13,
                            Title = b.Title,
                            Language = b.Language,
                            Binding = b.Binding,
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
    
    public void SaveBooks()
    {
        try
        {
            using var context = new BookstoreContext();
            var booksInDB = context.Books.Include(b => b.Authors).ToList();

            if (Books != null)
            {
                foreach (var book in Books)
                {
                    var bookFromDB = booksInDB.FirstOrDefault(b => b.Isbn13 == book.Isbn13);

                    if (bookFromDB != null)
                    {
                        bookFromDB.Title = book.Title;
                        bookFromDB.LanguageId = book.Language?.Id;
                        bookFromDB.Price = book.Price;
                        bookFromDB.DatePublished = book.DatePublished;
                        bookFromDB.BindingId = book.Binding.Id;
                        bookFromDB.Authors.Clear();

                        foreach (var author in book.Authors)
                        {
                            var retrievedAuthor = context.Authors.Find(author.Id);
                            if (retrievedAuthor != null)
                            {
                                bookFromDB.Authors.Add(retrievedAuthor);
                            }
                        }
                    }
                    else
                    {
                        var newBook = new Book()
                        {
                            Isbn13 = book.Isbn13,
                            Title = book.Title,
                            LanguageId = book.Language?.Id,
                            Price = book.Price,
                            DatePublished = book.DatePublished,
                            BindingId = book.Binding.Id,
                        };
                        foreach (var author in book.Authors)
                        {
                            var retrievedAuthor = context.Authors.Find(author.Id);
                            if (retrievedAuthor != null)
                            {
                                newBook.Authors.Add(retrievedAuthor);
                            }
                        }
                        context.Books.Add(newBook);
                    }
                }
            }
            foreach (var bookDB in booksInDB)
            {
                var book = Books?.FirstOrDefault(b => b.Isbn13 == bookDB.Isbn13);
                if (book == null)
                {
                    context.Books.Remove(bookDB);
                }
            }

                context.SaveChanges();
            
        }
        catch (Exception ex)
        {
        }
    }

    private async void GetAddableAuthors()
    {
        try
        {
            using var context = new BookstoreContext();

            if (SelectedBook != null)
            {
                var authors = await context.Authors.ToListAsync();

                var currentAuthors = SelectedBook.Authors.Select(a => a.Id).ToList();
                var addableAuthors = authors.Where(a => !currentAuthors.Contains(a.Id)).ToList();

                if (addableAuthors != null)
                {
                    AddableAuthors = new ObservableCollection<AuthorDisplay>(
                    addableAuthors.Select(a => new AuthorDisplay
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        BirthDate = a.BirthDate
                    }));
                }
            }
        }
        catch (Exception ex)
        {
     
        }
    }

    private bool IsValidISBN13(string isbn13)
    {
        if (isbn13.Length != 13)
        {
            IsbnMessege = "ISBN13 must contain 13 digits";
            return false;
        }

        if (!Regex.IsMatch(isbn13, @"^\d{13}$"))
        {
            IsbnMessege = "ISBN13 must contain 13 digits";
            return false;
        }

        if (!(isbn13.StartsWith("978") || isbn13.StartsWith("979")))
        {
            IsbnMessege = "Valid ISBN13 starts with 978 or 979";
            return false;
        }
        IsbnMessege = "";
        return true;
    }
}



