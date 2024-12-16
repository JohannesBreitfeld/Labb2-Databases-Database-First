using Bookstore.Domain;
using Bookstore.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Presentation.ViewModels;

public class AuthorViewModel : ViewModelBase
{
    private ObservableCollection<AuthorDisplay>? _authors;
    private AuthorDisplay? _selectedAuthor;

    public AuthorDisplay? SelectedAuthor
    {
        get => _selectedAuthor; 
        set 
        { 
            _selectedAuthor = value;
            RaisePropertyChanged();
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

    public AuthorViewModel()
    {
        GetAuthors();
    }

    private void GetAuthors()
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

        }
        catch (Exception ex)
        {
            //Statusmessege = $"Connecting to database failed : {ex.Message}";
        }
    }
}
