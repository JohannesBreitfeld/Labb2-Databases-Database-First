using Bookstore.Domain;
using Bookstore.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Bookstore.Presentation.ViewModels
{
    internal class InventoryViewModel : ViewModelBase
    {
        private Store? _selectedStore;
        private ObservableCollection<InventoryDisplay>? _inventories;
        private InventoryDisplay _selectedBook = null!;
        private ObservableCollection<InventoryDisplay>? _addableBooks;
        private InventoryDisplay _selectedBookToAdd = null!;

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
                _selectedStore = value;
                RaisePropertyChanged();
                LoadInventories();
                GetAddableBooks();
            }
        }

        public DelegateCommand SaveInventoryCommand { get; }
        public DelegateCommand RemoveBookCommand { get; }
        public DelegateCommand AddBookCommand { get; }

        public InventoryViewModel()
        {
            RemoveBookCommand = new DelegateCommand(RemoveBook, (object? _) => _selectedBook != null);
            SaveInventoryCommand = new DelegateCommand(SaveInventoryChangesToDB);
            AddBookCommand = new DelegateCommand(AddBook, (object? _) => SelectedBookToAdd != null);

            
            Stores = new ObservableCollection<Store>(GetStores()!);
            SelectedStore = Stores.FirstOrDefault();
            LoadInventories();
            
        }

        private void RemoveBook(object obj)
        {
            AddableBooks?.Add(SelectedBook);
            Inventories?.Remove(SelectedBook);
        }

        private void AddBook(object obj)
        {
            if (SelectedBookToAdd.Quantity >= 0)
            {
                Inventories?.Add(SelectedBookToAdd);
                AddableBooks?.Remove(SelectedBookToAdd);
            }
        }

        private List<Store>? GetStores()
        {
            using var context = new BookstoreContext();
            return context.Stores.Distinct().ToList();
        }

        private void LoadInventories()
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
        }

        private void SaveInventoryChangesToDB(object obj)
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
                        context.Inventories.Add(new Inventory() { 
                            Isbn13 = inventoryDisplay.ISBN13, 
                            StoreId = SelectedStore.Id, 
                            Quantity = inventoryDisplay.Quantity });
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
            }
        }
        
        private void GetAddableBooks()
        {
            using var context = new BookstoreContext();

            if (Inventories != null)
            {
                var books = context.Books.ToList();
                
                var inventoryIsbns = Inventories.Select(i => i.ISBN13).ToList();
                var addBooks = books.Where(b => !inventoryIsbns.Contains(b.Isbn13)).ToList();

                if(addBooks != null)
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
    }
}



//public void RemoveBookFromInventory(object obj)
//{
//    using var context = new BookstoreContext();
//    if (SelectedStore != null && obj is InventoryDisplay inventoryDisplay)
//    {
//        var inventory = context.Inventories
//            .FirstOrDefault(i => i.Isbn13 == inventoryDisplay.ISBN13 && i.StoreId == SelectedStore.Id);

//        if (inventory != null)
//        {
//            context.Inventories.Remove(inventory);
//            context.SaveChanges();
//            LoadInventories();
//        }
//    }
//}

//public void UpdateQuantity(object obj)
//{
//    var inventoryDisplay = obj as InventoryDisplay;
//    using var context = new BookStoreLightContext();

//    if (SelectedStore != null && inventoryDisplay != null)
//    {
//        var inventory = context.Inventories
//            .FirstOrDefault(i => i.Isbn13 == inventoryDisplay.ISBN13 && i.StoreId == SelectedStore.Id);

//        if (inventory != null)
//        {
//            inventory.Quantity = inventoryDisplay.Quantity;
//            context.SaveChanges();
//        }
//    }
//}