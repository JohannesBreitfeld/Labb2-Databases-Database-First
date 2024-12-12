using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Presentation.ViewModels
{
    internal class InventoryDisplay : ViewModelBase
    {
        private int _quantity = 0;
        public string ISBN13 { get; set; } = null!;
        public string Title { get; set; } = null!;
       
        public int Quantity 
        {
            get => _quantity;
            set
            {
                _quantity = value;
                RaisePropertyChanged();
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
