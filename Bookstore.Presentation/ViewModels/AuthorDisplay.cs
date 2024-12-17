using System.Collections.ObjectModel;

namespace Bookstore.Presentation.ViewModels;

public class AuthorDisplay : ViewModelBase
{
    private int _id;
    private string _firstName = null!;
    private string _lastName = null!;
    private DateOnly? _birthDate;
    private ObservableCollection<BookDisplay> _isbn13s = new ObservableCollection<BookDisplay>();

    private string? _titlesString;

    public string? TitlesString
    {
        get
        {
            string titles = string.Empty;

            foreach (var book in Isbn13s)
            {
                if (Isbn13s.Count == 1 || book == Isbn13s.LastOrDefault())
                {
                        titles += book.Title;
                }
                else
                {
                    titles += $"{book.Title}, ";
                }
            }

            TitlesString = titles;
            return titles;
        }
        set
        {
            if (_titlesString != value)
            {
                _titlesString = value;
                RaisePropertyChanged();
            }
        }
    }

    public event Action? AuthorPropertiesChanged;

    public int Id { get => _id; set => _id = value; }
    public string FirstName 
    { 
        get => _firstName;
        set
        {
            _firstName = value;
            RaisePropertyChanged();
            AuthorPropertiesChanged?.Invoke();
        }
    }
    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            RaisePropertyChanged();
            AuthorPropertiesChanged?.Invoke();
        }
    }
    public DateOnly? BirthDate
    {
        get => _birthDate;
        set
        {
            _birthDate = value;
            RaisePropertyChanged();
            AuthorPropertiesChanged?.Invoke();
        }
    }

    public virtual ObservableCollection<BookDisplay> Isbn13s 
    { 
        get => _isbn13s;
        set
        {
            _isbn13s = value;
            RaisePropertyChanged();
        }
    }
    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}
