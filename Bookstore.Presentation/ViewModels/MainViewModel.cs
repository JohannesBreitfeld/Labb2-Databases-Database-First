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
            
            if (value != InventoryViewModel)
            {
                InventoryViewModel!.SelectedStore = null;
            }
            else
            {
                InventoryViewModel!.SelectedStore = InventoryViewModel.Stores.FirstOrDefault();
            }

            if(value != BookCatalogViewModel && BookCatalogViewModel!.HasUnsavedChanges)
            {
                var result = UnsavedChangesMessegeBox();
           
                if (result == MessageBoxResult.Yes)
                {
                    BookCatalogViewModel!.SaveBooksAsync();
                }
                BookCatalogViewModel.HasUnsavedChanges = false;
            }
            if (value != AuthorViewModel && AuthorViewModel!.HasUnsavedChanges)
            { 
                var result = UnsavedChangesMessegeBox();

                if (result == MessageBoxResult.Yes)
                {
                    AuthorViewModel!.SaveAuthors();
                }
                AuthorViewModel.HasUnsavedChanges = false;
            }

            _selectedViewModel = value;
            RaisePropertyChanged();
        }
    }

    public MainViewModel(InventoryViewModel inventoryViewModel, BookCatalogViewModel bookCatalogViewModel, AuthorViewModel authorViewModel, Func<MessageBoxResult> unsavedChangesMessegeBox)
    {
        InventoryViewModel = inventoryViewModel;
        BookCatalogViewModel = bookCatalogViewModel;
        AuthorViewModel = authorViewModel;
        _selectedViewModel = InventoryViewModel;

        UnsavedChangesMessegeBox = unsavedChangesMessegeBox;

        SelectViewModelCommand = new DelegateCommand(SetViewModel);
    }

    public Func<MessageBoxResult> UnsavedChangesMessegeBox;

    private void SetViewModel(object obj)
    {
        if (obj is ViewModelBase viewModel)
        {
            SelectedViewModel = viewModel;
        }
    }

    public DelegateCommand SelectViewModelCommand { get; }



}
