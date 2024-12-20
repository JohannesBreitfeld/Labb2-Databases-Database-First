﻿using Bookstore.Domain;
using System.Collections.ObjectModel;

namespace Bookstore.Presentation.ViewModels;

public class BookDisplay : ViewModelBase
{
    private string? _authorsString;
    private string _isbn13 = string.Empty;
    private string _title = null!;
    private decimal _price = 0;
    private DateOnly? _datePublished;
    private BookBinding _binding = null!;
    private Language? _language;
    private ObservableCollection<AuthorDisplay> _authors = new ObservableCollection<AuthorDisplay>();

    public event Action? IsChanged;

    public string Isbn13 {
        get => _isbn13;
        set
        {
            if(value != _isbn13)
            {
                _isbn13 = value;
                 RaisePropertyChanged();
                 IsChanged?.Invoke();
            }
        }
    } 
    public string Title 
    { 
        get => _title; 
        set
        {
            if(value != _title)
            {
                _title = value;
                RaisePropertyChanged();
                IsChanged?.Invoke();
            }
        }
    } 
    public decimal Price 
    {
        get => _price;
        set
        {
            if(value != _price)
            {
                _price = value;
                RaisePropertyChanged();
                IsChanged?.Invoke();
            }
        }
    }
    public DateOnly? DatePublished 
    {
        get => _datePublished;
        set
        {
            if(value != _datePublished)
            {
                _datePublished = value;
                RaisePropertyChanged();
                IsChanged?.Invoke();
            }
        }
    }
    public  BookBinding Binding 
    {
        get => _binding;
        set
        {
            if(value != _binding)
            {
                _binding = value;
                RaisePropertyChanged();
                IsChanged?.Invoke();
            }
        }
    } 
    public  Language? Language 
    {
        get => _language;
        set
        {
            if(value != _language)
            {
                _language = value;
                IsChanged?.Invoke();
            }
            RaisePropertyChanged();
        }
    }
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
    public  ObservableCollection<AuthorDisplay> Authors {
        get => _authors;
        set
        {
            if(value != _authors)
            {
                _authors = value;
                RaisePropertyChanged();
                IsChanged?.Invoke();
            }
        }
    } 
    
}
