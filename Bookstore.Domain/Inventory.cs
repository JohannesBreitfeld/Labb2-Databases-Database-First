namespace Bookstore.Domain;

public partial class Inventory
{
    public string Isbn13 { get; set; } = null!;

    public int StoreId { get; set; }

    public int Quantity { get; set; }

    public virtual Book Isbn13Navigation { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
