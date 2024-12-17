using Bookstore.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bookstore.Presentation.Views
{
    /// <summary>
    /// Interaction logic for BookCatalogView.xaml
    /// </summary>
    public partial class BookCatalogView : UserControl
    {
        public BookCatalogView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            Grid.SetColumn(GridCursor, index % 10);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (DataContext is BookCatalogViewModel viewModel)
            {
                await viewModel.GetBooksAsync();
            }
        }
    }
    
}
