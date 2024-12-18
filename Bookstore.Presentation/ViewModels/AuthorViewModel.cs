using Bookstore.Domain;
using Bookstore.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Bookstore.Presentation.ViewModels;

public class AuthorViewModel : ViewModelBase
{
    private ObservableCollection<AuthorDisplay>? _authors;
    private AuthorDisplay? _selectedAuthor;
    private AuthorDisplay _authorToAdd = null!;
    private string _statusMessage = null!;
    private bool _hasUnsavedChanges = false;

    public override bool HasUnsavedChanges
    {
        get => _hasUnsavedChanges; 
        set 
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
            RaisePropertyChanged();
            SaveAuthorsCommand.RaiseCanExecuteChanged();
            UndoChangesCommand.RaiseCanExecuteChanged();
        }
    }
    public string StatusMessage
    {
        get => _statusMessage; 
        set 
        { 
            _statusMessage = value;
            RaisePropertyChanged();
        }
    }
    public AuthorDisplay? SelectedAuthor
    {
        get => _selectedAuthor; 
        set 
        { 
            _selectedAuthor = value;
            RaisePropertyChanged();
            DeleteAuthorCommand.RaiseCanExecuteChanged();
        }
    }
    public ObservableCollection<AuthorDisplay>? Authors 
    { 
        get => _authors;
        set
        {
            _authors = value;
            if (_authors != null)
            {
                _isLoading = true;
                foreach (var author in _authors)
                {
                    author.AuthorPropertiesChanged += OnPropertyChanged;
                }
                _isLoading = false;
            }
            RaisePropertyChanged();
        }
    }

    public DelegateCommand AddAuthorCommand { get; }
    public DelegateCommand DeleteAuthorCommand { get; }
    
    public DelegateCommandAsync SaveAuthorsCommand { get; }
    public DelegateCommandAsync UndoChangesCommand { get; }

    public AuthorDisplay AuthorToAdd 
    { 
        get => _authorToAdd;
        set
        {
            _authorToAdd = value;
            RaisePropertyChanged();
        }
    }

    public AuthorViewModel()
    {
        AuthorToAdd = new AuthorDisplay();
        
        AddAuthorCommand = new DelegateCommand(AddAuthor);
        DeleteAuthorCommand = new DelegateCommand(RemoveAuthor, (object? _) => SelectedAuthor != null);
        
        SaveAuthorsCommand = new DelegateCommandAsync(SaveAuthorsAsync, (object? _) => HasUnsavedChanges == true);
        UndoChangesCommand = new DelegateCommandAsync(GetAuthorsAsync, (object? _) => HasUnsavedChanges == true);
    }

    private void RemoveAuthor(object obj)
    {
        if (SelectedAuthor != null)
        {
            SelectedAuthor.AuthorPropertiesChanged -= OnPropertyChanged;
            Authors?.Remove(SelectedAuthor!);
            HasUnsavedChanges = true;
        }
    }

    private void AddAuthor(object obj)
    {
        if(AuthorToAdd.FirstName != null && AuthorToAdd.LastName != null)
        {
            Authors?.Add(AuthorToAdd);
            AuthorToAdd = new AuthorDisplay();
            HasUnsavedChanges = true;
        }
    }

    public async Task GetAuthorsAsync(object obj = null!)
    {
        try
        {
            using var context = new BookstoreContext();
            var authors =  await context.Authors.Include(a => a.Isbn13s).Distinct().ToListAsync();

            Authors = new ObservableCollection<AuthorDisplay>(
                    authors.Select(a => new AuthorDisplay
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        BirthDate = a.BirthDate,
                        Isbn13s = new ObservableCollection<BookDisplay>(a.Isbn13s
                                .Select(b => new BookDisplay()
                                {
                                    Isbn13 = b.Isbn13,
                                    Title = b.Title,
                                    Binding = b.Binding,
                                    Price = b.Price
                                }))
                    }));
            HasUnsavedChanges = false;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Connecting to database failed : {ex.Message}";
        }
    }

    public async Task SaveAuthorsAsync(object obj = null!)
    {
        try
        {
            using var context = new BookstoreContext();
            var authors = await context.Authors.Distinct().ToListAsync();
            
            if (Authors != null)
            {
                foreach (var author in Authors)
                {
                    var authorDB = authors.FirstOrDefault(a => a.Id == author.Id);

                    if(authorDB != null)
                    {
                        authorDB.FirstName = author.FirstName;
                        authorDB.LastName = author.LastName;
                        authorDB.BirthDate = author.BirthDate;
                    }
                    else
                    {
                        context.Authors.Add(new Author()
                        {
                            FirstName = author.FirstName,
                            LastName = author.LastName,
                            BirthDate = author.BirthDate
                        });
                    }

                }
                foreach (var author in authors)
                {
                    var authorNotInDB = Authors.FirstOrDefault(a => a.Id == author.Id);
                    if(authorNotInDB == null)
                    {
                        context.Authors.Remove(author);
                    }
                }
            }
            HasUnsavedChanges = false;
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Connecting to database failed : {ex.Message}";
        }
    }
}
