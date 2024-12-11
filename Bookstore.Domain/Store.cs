namespace Bookstore.Domain;

public partial class Store
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public override string ToString()
    {
        return Name;
    }
}
