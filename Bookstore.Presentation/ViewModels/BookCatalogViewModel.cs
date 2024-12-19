using Bookstore.Domain;
using Bookstore.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Bookstore.Presentation.ViewModels;

public class BookCatalogViewModel : ViewModelBase
{
	private ObservableCollection<BookDisplay>? _books;
    private BookDisplay? _selectedBook;
    private AuthorDisplay? _selectedAuthor;
    private AuthorDisplay? _authorToAdd;
    private ObservableCollection<Language>? _languages;
    private string _isbnMessege = string.Empty;
    private ObservableCollection<AuthorDisplay>? _addableAuthors;
    private string _selectedBookIsbn = null!;
    private ObservableCollection<BookBinding> bindings = null!;
    private bool _hasUnsavedChanges;
    private bool _showEditMode = true;
    private bool _showAddMode = false;

    public BookDisplay BookToAdd { get; set; }
    public string? StatusMessage { get; set; }
    public string? AddBookMessage { get; set; }
    
    public ObservableCollection<BookBinding> Bindings 
    {
        get => bindings;
        set
        {
            bindings = value;
            RaisePropertyChanged();
        }
    }
    public override bool HasUnsavedChanges
    {
        get => _hasUnsavedChanges;
        set
        {
            if (_hasUnsavedChanges != value)
            {
                _hasUnsavedChanges = value;

                if (value == true)
                {
                    StatusMessage = "You have unsaved changes";
                }
                else
                {
                    StatusMessage = "";
                }

                RaisePropertyChanged(nameof(StatusMessage));
                RaisePropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
                UndoChangesCommand.RaiseCanExecuteChanged();
            }
        }
    }
    public bool ShowEditMode 
    { 
        get => _showEditMode;
        set
        {
            if(!value == _showEditMode)
            {
                _showEditMode = value;
                RaisePropertyChanged();
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
                _showAddMode = value;
                RaisePropertyChanged();
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
    public string IsbnMessage 
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
    public ObservableCollection<Language>? Languages 
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
            SelectedAuthor = _selectedBook?.Authors.FirstOrDefault();
            RemoveAuthorCommand.RaiseCanExecuteChanged();
            RemoveBookCommand.RaiseCanExecuteChanged();
            AddBookToCatalogCommand.RaiseCanExecuteChanged();
            GetAddableAuthors();
            if(SelectedBook != null)
            {
                _isLoading = true;
           
                SelectedBookIsbn = SelectedBook.Isbn13;
                SelectedBook.Language = Languages?.FirstOrDefault(l => l?.Id == SelectedBook?.Language?.Id);
                if (SelectedBook.Binding != null)
                {
                    SelectedBook.Binding = Bindings.First(b => b.Id == SelectedBook.Binding.Id);
                }
              
                _isLoading = false;
            }
        }
    }
    public AuthorDisplay? SelectedAuthor
    {
        get => _selectedAuthor;
        set
        {
            _selectedAuthor = value;
            RaisePropertyChanged();
            RemoveAuthorCommand.RaiseCanExecuteChanged();
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
   
    public DelegateCommand AddAuthorCommand { get; }
    public DelegateCommand RemoveAuthorCommand { get; } 
    public DelegateCommand RemoveBookCommand { get; }
    public DelegateCommand AddModeCommand { get; }
    public DelegateCommand EditModeCommand { get; }
    public DelegateCommand AddBookToCatalogCommand { get; }
    
    public DelegateCommandAsync SaveCommand { get; }
    public DelegateCommandAsync UndoChangesCommand { get; }

    public BookCatalogViewModel()
    {
        BookToAdd = new BookDisplay();
        RemoveAuthorCommand = new DelegateCommand(RemoveAuthorFromBook, (object? _) => SelectedAuthor != null);
        AddAuthorCommand = new DelegateCommand(AddAuthorToBook, (object? _) => AuthorToAdd != null);
        SaveCommand = new DelegateCommandAsync(SaveBooksAsync, (object? _) => HasUnsavedChanges == true);
        RemoveBookCommand = new DelegateCommand(RemoveBookFromCatalog, (object? _) => SelectedBook != null);
        AddModeCommand = new DelegateCommand(ChangeToAddMode);
        EditModeCommand = new DelegateCommand(ChangeToEditMode);
        UndoChangesCommand = new DelegateCommandAsync(GetBooksAsync, (object? _) => HasUnsavedChanges == true);
        AddBookToCatalogCommand = new DelegateCommand(AddBookToCatalog);
    }

    private void AddBookToCatalog(object obj)
    {
        if (BookToAdd == null)
        {
            return;
        }
        if (!IsValidISBN13(BookToAdd!.Isbn13))
        {
            AddBookMessage = "Invalid ISBN13";
            RaisePropertyChanged(nameof(AddBookMessage));
            return;
        }
        if (BookToAdd.Price < 0)
        {
            AddBookMessage = "Books must be assigned a non negative price";
            RaisePropertyChanged(nameof(AddBookMessage));
            return;
        }
        if (BookToAdd.Binding == null)
        {
            AddBookMessage = "Books must be assigned a binding";
            RaisePropertyChanged(nameof(AddBookMessage));
            return;
        }
        AddBookMessage = string.Empty;
        RaisePropertyChanged(nameof(AddBookMessage));
        Books?.Add(BookToAdd);
        HasUnsavedChanges = true;
        BookToAdd = new BookDisplay();
    }

    private void ChangeToEditMode(object obj)
    {
        if (!ShowEditMode)
        {
            ShowEditMode = true;
            ShowAddMode = false;
            IsbnMessage = "";
            SelectedBook = Books?.FirstOrDefault();
        }
    }

    private void ChangeToAddMode(object obj)
    {
       if(!ShowAddMode)
        {
            ShowAddMode = true;
            ShowEditMode = false;
            IsbnMessage = "";
            SelectedBook = BookToAdd;
        }
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
        if (SelectedBook != null && SelectedAuthor != null)
        {
            SelectedBook.Authors.Remove(SelectedAuthor);
            SelectedBook.RaisePropertyChanged(nameof(SelectedBook.AuthorsString));
            
            if (SelectedBook != BookToAdd)
            {
                HasUnsavedChanges = true;
            }
        }
    }

    private void AddAuthorToBook(object obj)
    {
        if (SelectedBook != null && AuthorToAdd != null)
        {
            SelectedBook.Authors.Add(AuthorToAdd);
            SelectedBook.RaisePropertyChanged(nameof(SelectedBook.AuthorsString));
            GetAddableAuthors();

            if (SelectedBook != BookToAdd)
            {
                HasUnsavedChanges = true;
            }
        }
    }

    private async Task GetLanguagesAsync()
    {
        using (var context = new BookstoreContext())
        {
            Languages = new ObservableCollection<Language>(await context.Languages.Distinct().ToListAsync());
        }
    }

    private async Task GetBindingsAsync()
    {
        using (var context = new BookstoreContext())
        {
            Bindings = new ObservableCollection<BookBinding>(await context.BookBindings.Distinct().ToListAsync());
        }
    }

    public async Task GetBooksAsync(object obj = null!)
	{
        await GetLanguagesAsync();
        await GetBindingsAsync();
        using var context = new BookstoreContext();

        var books = new ObservableCollection<BookDisplay>
        (
           await context.Books
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
               })
               .ToListAsync()  
       );
        HasUnsavedChanges = false;
        Books = books;
    }
    
    public async Task SaveBooksAsync(object obj = null!)
    {
        try
        {
            using var context = new BookstoreContext();
            var booksInDB = await context.Books.Include(b => b.Authors).Include(b => b.Inventories).ToListAsync();

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
                            var retrievedAuthor = await context.Authors.FindAsync(author.Id);
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
                            var retrievedAuthor = await context.Authors.FindAsync(author.Id);
                            if (retrievedAuthor != null)
                            {
                                newBook.Authors.Add(retrievedAuthor);
                            }
                        }
                        await context.Books.AddAsync(newBook);
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
            await context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            StatusMessage = $"Could not save to database : {ex.Message}";
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
                var addableAuthorsExistingBook = authors.Where(a => !currentAuthors.Contains(a.Id)).ToList();



                if (addableAuthorsExistingBook != null)
                {
                    AddableAuthors = new ObservableCollection<AuthorDisplay>(
                    addableAuthorsExistingBook.Select(a => new AuthorDisplay
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
            StatusMessage = $"Could not connect to database : {ex.Message}";
        }
    }

    private bool IsValidISBN13(string isbn13)
    {
        if (isbn13.Length != 13)
        {
            IsbnMessage = "ISBN13 must contain 13 digits";
            return false;
        }

        if (!Regex.IsMatch(isbn13, @"^\d{13}$"))
        {
            IsbnMessage = "ISBN13 must contain 13 digits";
            return false;
        }

        if (!(isbn13.StartsWith("978") || isbn13.StartsWith("979")))
        {
            IsbnMessage = "Valid ISBN13 starts with 978 or 979";
            return false;
        }
        IsbnMessage = "";
        return true;
    }
}



