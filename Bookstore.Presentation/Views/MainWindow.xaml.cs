using Bookstore.Presentation.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            this.BookCatalogViewModel = new BookCatalogViewModel(UnsavedChanges);
            this.AuthorViewModel = new AuthorViewModel();

            DataContext = new MainViewModel(InventoryViewModel, BookCatalogViewModel, AuthorViewModel);
        }
       
        
        public MessageBoxResult UnsavedChanges()
        {
            var result = MessageBox.Show("You have unsaved changes that will be lost. Would you like to save before continuing?", "Warning", MessageBoxButton.YesNo);
         
            return result;
        }
    }
}