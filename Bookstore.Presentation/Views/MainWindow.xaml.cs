using Bookstore.Presentation.ViewModels;
using System.Windows;

namespace Bookstore.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public InventoryViewModel InventoryViewModel { get; }
        public BookCatalogViewModel BookCatalogViewModel { get; }
        public AuthorViewModel AuthorViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            
            this.InventoryViewModel = new InventoryViewModel(UnsavedChanges);
            this.BookCatalogViewModel = new BookCatalogViewModel();
            this.AuthorViewModel = new AuthorViewModel();

            DataContext = new MainViewModel(InventoryViewModel, BookCatalogViewModel, AuthorViewModel, UnsavedChanges);
        }
       
        public MessageBoxResult UnsavedChanges()
        {
            var result = MessageBox.Show("You have unsaved changes that will be lost. Would you like to save before continuing?", "Warning", MessageBoxButton.YesNo);
         
            return result;
        }
    }
}