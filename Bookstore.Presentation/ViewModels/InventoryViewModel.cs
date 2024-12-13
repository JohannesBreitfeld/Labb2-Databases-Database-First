using Bookstore.Domain;
using Bookstore.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace Bookstore.Presentation.ViewModels;

public class InventoryViewModel : ViewModelBase
{
    private bool _isLoading = true;
    private Store? _selectedStore;
    private ObservableCollection<InventoryDisplay>? _inventories;
    private InventoryDisplay _selectedBook = null!;
    private ObservableCollection<InventoryDisplay>? _addableBooks;
    private InventoryDisplay _selectedBookToAdd = null!;
    private bool _hasUnsavedChanges;
   
    public InventoryViewModel(Func<MessageBoxResult> unsavedChangesMessegeBox)
    {
        UnsavedChangesMessegeBox = unsavedChangesMessegeBox;
        RemoveBookCommand = new DelegateCommand(RemoveBook, (object? _) => _selectedBook != null);
        SaveInventoryCommand = new DelegateCommand(SaveInventoryChangesToDB, (object? _) => HasUnsavedChanges);
        AddBookCommand = new DelegateCommand(AddBook, (object? _) => SelectedBookToAdd != null);
        UndoChangesCommand = new DelegateCommand((object _) => LoadInventories(), (object? _) => HasUnsavedChanges);
        
        Stores = new ObservableCollection<Store>(GetStores()!);
        SelectedStore = Stores.FirstOrDefault();
        LoadInventories();
        
    }

    public string? Statusmessege { get; set; } 
    public ObservableCollection<Store> Stores { get; set; }

    public InventoryDisplay SelectedBookToAdd
    {
        get =>  _selectedBookToAdd;
        set
        {
            _selectedBookToAdd = value;
            RaisePropertyChanged();
            AddBookCommand.RaiseCanExecuteChanged();
        }
    }
    public ObservableCollection<InventoryDisplay>? AddableBooks
    {
        get => _addableBooks; 
        set 
        {
            _addableBooks = value;
            RaisePropertyChanged();
        }
    }
    public ObservableCollection<InventoryDisplay>? Inventories
    {
        get => _inventories;
        set
        {
            _inventories = value;
            RaisePropertyChanged();
            if (_inventories != null)
            {
                _isLoading = true;
                foreach (var inventoryDisplay in _inventories)
                {
                    inventoryDisplay.QuantityChanged += OnQuantityChanged;
                }
                _isLoading = false;
            }
        }
    }
    public InventoryDisplay SelectedBook
    {
        get => _selectedBook; 
        set
        {
            _selectedBook = value;
            RaisePropertyChanged();
            RemoveBookCommand.RaiseCanExecuteChanged();
        }
    }
    public Store? SelectedStore
    {
        get => _selectedStore;
        set
        {
            if (_selectedStore != value)
            {
                if (_hasUnsavedChanges)
                {
                    var result = UnsavedChangesMessegeBox();
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        SaveInventoryChangesToDB(null!);
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        HasUnsavedChanges = false;
                    }
                }
                _selectedStore = value;
                RaisePropertyChanged();
                LoadInventories();
                GetAddableBooks();
            }
        }
    }
    public bool HasUnsavedChanges
    {
        get => _hasUnsavedChanges;
        set
        {
            if (_hasUnsavedChanges != value)
            {
                _hasUnsavedChanges = value;
                
                if (value == true)
                {
                    Statusmessege = "You have unsaved changes.";
                }
                else
                {
                    Statusmessege = "";
                }

                RaisePropertyChanged(nameof(Statusmessege));
                RaisePropertyChanged();
                SaveInventoryCommand.RaiseCanExecuteChanged();
                UndoChangesCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public Func<MessageBoxResult> UnsavedChangesMessegeBox;
    public DelegateCommand SaveInventoryCommand { get; }
    public DelegateCommand RemoveBookCommand { get; }
    public DelegateCommand AddBookCommand { get; }
    public DelegateCommand UndoChangesCommand { get; }

    private void OnQuantityChanged()
    {
        if (!_isLoading)
        {
            HasUnsavedChanges = true;
        }
    }

    private void RemoveBook(object obj)
    {
        SelectedBook.QuantityChanged -= OnQuantityChanged;
        AddableBooks?.Add(SelectedBook);
        Inventories?.Remove(SelectedBook);
        HasUnsavedChanges = true;
    }

    private void AddBook(object obj)
    {
        if (SelectedBookToAdd.Quantity >= 0)
        {
            Inventories?.Add(SelectedBookToAdd);
            AddableBooks?.Remove(SelectedBookToAdd);
            HasUnsavedChanges = true;
        }
    }

    private List<Store>? GetStores()
    {
        try
        {
            using var context = new BookstoreContext();
            return context.Stores.Distinct().ToList();
        }
        catch (Exception ex)
        {
            Statusmessege = $"Connecting to database failed : {ex.Message}";
            return null;
        }
    }

    private void LoadInventories()
    {
        try
        {
            if (SelectedStore != null)
            {
                using var context = new BookstoreContext();
                var inventories = context.Inventories
                                .Where(i => i.StoreId == SelectedStore.Id)
                                .Include(i => i.Isbn13Navigation)
                                .ToList();
                Inventories = new ObservableCollection<InventoryDisplay>(
                    inventories.Select(i => new InventoryDisplay
                    {
                        ISBN13 = i.Isbn13,
                        Title = i.Isbn13Navigation.Title,
                        Quantity = i.Quantity
                    }));
            }
            else
            {
                Inventories!.Clear();
            }
            HasUnsavedChanges = false;
        }
        catch (Exception ex)
        {
            Statusmessege = $"Connecting to database failed : {ex.Message}";
        }
    }

    public void SaveInventoryChangesToDB(object obj)
    {
        try
        {
            using var context = new BookstoreContext();

            var inventoryDisplays = Inventories;

            if (SelectedStore != null && inventoryDisplays != null)
            {
                var inventories = context.Inventories
                    .Where(i => i.StoreId == SelectedStore.Id)
                    .ToList();

                foreach (var inventoryDisplay in inventoryDisplays)
                {
                    var inventory = inventories.FirstOrDefault(i => i.Isbn13 == inventoryDisplay.ISBN13);

                    if (inventory != null)
                    {
                        inventory.Quantity = inventoryDisplay.Quantity;
                    }
                    else
                    {
                        context.Inventories.Add(new Inventory()
                        {
                            Isbn13 = inventoryDisplay.ISBN13,
                            StoreId = SelectedStore.Id,
                            Quantity = inventoryDisplay.Quantity
                        });
                    }
                }
                foreach (var inventory in inventories)
                {
                    var book = inventoryDisplays.FirstOrDefault(i => i.ISBN13 == inventory.Isbn13);
                    if (book == null)
                    {
                        context.Inventories.Remove(inventory);
                    }
                }

                context.SaveChanges();
                HasUnsavedChanges = false;
            }
        }
        catch(Exception ex)
        {
            Statusmessege = $"Connecting to database failed : {ex.Message}";
        }
    }

    private void GetAddableBooks()
    {
        try
        {
            using var context = new BookstoreContext();

            if (Inventories != null)
            {
                var books = context.Books.ToList();

                var inventoryIsbns = Inventories.Select(i => i.ISBN13).ToList();
                var addBooks = books.Where(b => !inventoryIsbns.Contains(b.Isbn13)).ToList();

                if (addBooks != null)
                {
                    AddableBooks = new ObservableCollection<InventoryDisplay>(
                    addBooks.Select(i => new InventoryDisplay
                    {
                        ISBN13 = i.Isbn13,
                        Title = i.Title,
                        Quantity = 1
                    }));
                }
            }
            else
            {
                var addBooks = context.Books.ToList();

                if (addBooks != null)
                {
                    AddableBooks = new ObservableCollection<InventoryDisplay>(
                    addBooks.Select(i => new InventoryDisplay
                    {
                        ISBN13 = i.Isbn13,
                        Title = i.Title,
                        Quantity = 1
                    }));
                }
            }
        }
        catch(Exception ex)
        {
            Statusmessege = $"Connecting to database failed : {ex.Message}";
        }
    }
}
