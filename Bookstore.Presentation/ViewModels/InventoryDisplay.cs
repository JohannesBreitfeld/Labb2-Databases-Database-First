using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Presentation.ViewModels
{
    internal class InventoryDisplay
    {
        public string ISBN13 { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int Quantity { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
