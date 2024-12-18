using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bookstore.Presentation.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase? _selectedViewModel;

    public AuthorViewModel? AuthorViewModel { get; }
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
    
    public DelegateCommandAsync SelectViewModelCommand { get; }
    public Func<MessageBoxResult> UnsavedChangesMessegeBox;

    public MainViewModel(InventoryViewModel inventoryViewModel, 
        BookCatalogViewModel bookCatalogViewModel, AuthorViewModel authorViewModel, Func<MessageBoxResult> unsavedChangesMessegeBox)
    {
        InventoryViewModel = inventoryViewModel;
        BookCatalogViewModel = bookCatalogViewModel;
        AuthorViewModel = authorViewModel;
        _selectedViewModel = InventoryViewModel;

        UnsavedChangesMessegeBox = unsavedChangesMessegeBox;

        SelectViewModelCommand = new DelegateCommandAsync(SetViewModel);
    }

    private async Task SetViewModel(object obj)
    {
        if (obj is ViewModelBase viewModel)
        {
            if (viewModel != InventoryViewModel)
            {
                InventoryViewModel!.SelectedStore = null;
            }
            else
            {
                InventoryViewModel!.SelectedStore = InventoryViewModel.Stores.FirstOrDefault();
            }

            if (viewModel != BookCatalogViewModel && BookCatalogViewModel!.HasUnsavedChanges)
            {
                var result = UnsavedChangesMessegeBox();

                if (result == MessageBoxResult.Yes)
                {
                   await BookCatalogViewModel!.SaveBooksAsync();
                }
                BookCatalogViewModel.HasUnsavedChanges = false;
            }
            if (viewModel != AuthorViewModel && AuthorViewModel!.HasUnsavedChanges)
            {
                var result = UnsavedChangesMessegeBox();

                if (result == MessageBoxResult.Yes)
                {
                    await AuthorViewModel!.SaveAuthorsAsync();
                }
                AuthorViewModel.HasUnsavedChanges = false;
            }
            SelectedViewModel = viewModel;
        }
    }
}
