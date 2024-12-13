using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Presentation.ViewModels;

internal class MainViewModel : ViewModelBase
{
    private ViewModelBase? _selectedViewModel;

    public InventoryViewModel? InventoryViewModel { get; }
    public BookCatalogViewModel? BookCatalogViewModel { get; }
    public ViewModelBase? SelectedViewModel
    {
        get => _selectedViewModel;
        set
        {
            _selectedViewModel = value;
            RaisePropertyChanged();
        }
    }

    public MainViewModel(InventoryViewModel inventoryViewModel, BookCatalogViewModel bookCatalogViewModel)
    {
        InventoryViewModel = inventoryViewModel;
        BookCatalogViewModel = bookCatalogViewModel;
        _selectedViewModel = InventoryViewModel;

        SelectViewModelCommand = new DelegateCommand(SetViewModel);
    }

    private void SetViewModel(object obj)
    {
        if (obj is ViewModelBase viewModel)
        {
            SelectedViewModel = viewModel;
        }
    }

    public DelegateCommand SelectViewModelCommand { get; }



}
