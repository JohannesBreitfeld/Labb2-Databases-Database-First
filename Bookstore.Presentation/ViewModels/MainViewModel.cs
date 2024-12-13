using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Presentation.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private ViewModelBase? _selectedViewModel;

        public InventoryViewModel? InventoryViewModel { get; }
        public ViewModelBase? SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                _selectedViewModel = value;
                RaisePropertyChanged();
            }
        }

        public MainViewModel(InventoryViewModel inventoryViewModel)
        {
            InventoryViewModel = inventoryViewModel;
            _selectedViewModel = InventoryViewModel;
        }
    }
}
