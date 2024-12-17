using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bookstore.Presentation.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
    public bool _isLoading = false;

    public virtual bool HasUnsavedChanges { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public void OnPropertyChanged()
    {
        if (!_isLoading)
        {
            HasUnsavedChanges = true;
        }
    }
}
