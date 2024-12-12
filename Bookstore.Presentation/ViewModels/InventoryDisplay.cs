using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Presentation.ViewModels
{
    internal class InventoryDisplay : ViewModelBase
    {
        private int _quantity;
        public string ISBN13 { get; set; } = null!;
        public string Title { get; set; } = null!;

        public event Action QuantityChanged;

        public int Quantity 
        {
            get => _quantity;
            set
            {
                if (_quantity != value) 
                {
                    _quantity = value;
                    RaisePropertyChanged();

                    QuantityChanged?.Invoke();
                }
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
