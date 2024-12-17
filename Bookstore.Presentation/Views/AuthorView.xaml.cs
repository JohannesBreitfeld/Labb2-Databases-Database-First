using Bookstore.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Bookstore.Presentation.Views
{
    /// <summary>
    /// Interaction logic for AuthorView.xaml
    /// </summary>
    public partial class AuthorView : UserControl
    {
        public AuthorView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AuthorViewModel viewModel)
            {
                await viewModel.GetAuthors();
            }
        }
    }
    
}
