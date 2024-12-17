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
    private string _statusMessage;
    private bool _hasUnsavedChanges;

    public bool HasUnsavedChanges
    {
        get { return _hasUnsavedChanges; }
        set { _hasUnsavedChanges = value; }
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
            RaisePropertyChanged();
        }
    }

    public DelegateCommand SaveAuthorsCommand { get; }
    public DelegateCommand AddAuthorCommand { get; }
    public DelegateCommand DeleteAuthorCommand { get; }

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
        GetAuthors();
        AuthorToAdd = new AuthorDisplay();
        AddAuthorCommand = new DelegateCommand(AddAuthor);
        SaveAuthorsCommand = new DelegateCommand(SaveAuthors);
        DeleteAuthorCommand = new DelegateCommand((object _) => Authors?.Remove(SelectedAuthor!), (object? _) => SelectedAuthor != null);
    }

    private void AddAuthor(object obj)
    {
        if(AuthorToAdd.FirstName != null && AuthorToAdd.LastName != null)
        {
            Authors?.Add(AuthorToAdd);
            AuthorToAdd = new AuthorDisplay();
        }
    }



    private void GetAuthors(object obj = null!)
    {
        try
        {
            using var context = new BookstoreContext();
            var authors = context.Authors.Include(a => a.Isbn13s).Distinct().ToList();

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
    private void SaveAuthors(object obj = null!)
    {
        try
        {
            using var context = new BookstoreContext();
            var authors = context.Authors.Distinct().ToList();
            
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
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Connecting to database failed : {ex.Message}";
        }
    }
}
