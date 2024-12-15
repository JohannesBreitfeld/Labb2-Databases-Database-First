using Bookstore.Domain;
using Bookstore.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace Bookstore.Presentation.ViewModels;

public class BookCatalogViewModel : ViewModelBase
{
	private ObservableCollection<BookDisplay>? _books;
    private BookDisplay? _selectedBook;
    private AuthorDisplay? _authorToAdd;
    private ObservableCollection<Language?>? _languages;
    private string _isbnMessege = string.Empty;
    private ObservableCollection<AuthorDisplay>? _addableAuthors;
    private string _selectedBookIsbn = null!;
    private bool _isLoading = true;
    private bool _hasUnsavedChanges;
    private bool _showEditMode = true;

    public bool HasUnsavedChanges
    {
        get => _hasUnsavedChanges;
        set
        {
            if (_hasUnsavedChanges != value)
            {
                _hasUnsavedChanges = value;

                if (value == true)
                {
                    StatusMessege = "You have unsaved changes";
                }
                else
                {
                    StatusMessege = "";
                }

                RaisePropertyChanged(nameof(StatusMessege));
                RaisePropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
                //UndoChangesCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string StatusMessege { get; set; }
    public bool ShowEditMode 
    { 
        get => _showEditMode;
        set
        {
            if(!value == _showEditMode)
            {
                //if (_hasUnsavedChanges)
                //{
                //    var result = UnsavedChangesMessegeBox();

                //    if (result == MessageBoxResult.Yes)
                //    {
                //        SaveBooks(null!);
                //    }
                //    else if (result == MessageBoxResult.No)
                //    {
                //        HasUnsavedChanges = false;
                //    }
                //}
            _showEditMode = value;
            }
        }

    }
    public bool ShowAddMode 
    { 
        get => _showAddMode; 
        set 
        {
            if (!value == _showAddMode)
            {
                //if (_hasUnsavedChanges)
                //{
                //    var result = UnsavedChangesMessegeBox();

                //    if (result == MessageBoxResult.Yes)
                //    {
                //        SaveBooks(null!);
                //    }
                //    else if (result == MessageBoxResult.No)
                //    {
                //        HasUnsavedChanges = false;
                //    }
                //}
                _showAddMode = value;
            }
        }
    }
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

            RaisePropertyChanged();
            RemoveAuthorCommand.RaiseCanExecuteChanged();
            RemoveBookCommand.RaiseCanExecuteChanged();
            GetAddableAuthors();
            if(SelectedBook != null)
            {
                _isLoading = true;
           
                SelectedBookIsbn = SelectedBook.Isbn13;
                SelectedBook.Language = Languages?.FirstOrDefault(l => l?.Id == SelectedBook.Language.Id);
                SelectedBook.Binding = Bindings.First(b => b.Id == SelectedBook.Binding.Id);
              
                _isLoading = false;
                

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
            if (_books != null)
            {
                _isLoading = true;
                foreach (var book in _books)
                {
                    book.IsChanged += OnPropertyChanged;
                }
                _isLoading = false;
            }
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
    public ObservableCollection<BookBinding> Bindings { get; set; } = null!;
    
    public Func<MessageBoxResult> UnsavedChangesMessegeBox;
    private bool _showAddMode = false;

    public DelegateCommand SaveCommand { get; }
    public DelegateCommand AddAuthorCommand { get; }
    public DelegateCommand RemoveAuthorCommand { get; }
    public DelegateCommand RemoveBookCommand { get; }
    public DelegateCommand ChangeModeCommand { get; }

    public BookCatalogViewModel(Func<MessageBoxResult> unsavedChangesPrompt)
    {
        GetBooks();
        UnsavedChangesMessegeBox = unsavedChangesPrompt;
        RemoveAuthorCommand = new DelegateCommand(RemoveAuthorFromBook, (object? _) => SelectedBook?.SelectedAuthor != null);
        AddAuthorCommand = new DelegateCommand(AddAuthorToBook, (object? _) => AuthorToAdd != null);
        SaveCommand = new DelegateCommand(SaveBooks, (object? _) => HasUnsavedChanges == true);
        RemoveBookCommand = new DelegateCommand(RemoveBookFromCatalog, (object? _) => SelectedBook != null);
        ChangeModeCommand = new DelegateCommand(ChangeMode);
    }

    private void ChangeMode(object obj)
    {
        if(ShowAddMode == true)
        {
            ShowAddMode = false;
            ShowEditMode = true;
        }
        else
        {
            ShowAddMode = true;
            ShowEditMode = false;
        }
        RaisePropertyChanged(nameof(ShowAddMode));
        RaisePropertyChanged(nameof(ShowEditMode));
    }

    private void RemoveBookFromCatalog(object obj = null!)
    {
        if (SelectedBook != null)
        {
            SelectedBook.IsChanged -= OnPropertyChanged;
            Books?.Remove(SelectedBook!);
            HasUnsavedChanges = true;
        }
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
    
    public void SaveBooks(object obj = null!)
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
            HasUnsavedChanges = false;
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

    private void OnPropertyChanged()
    {
        if (!_isLoading)
        {
            HasUnsavedChanges = true;
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



