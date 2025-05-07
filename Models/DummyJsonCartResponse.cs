public class DummyJsonCartResponse
{
    public List<Cart> Carts { get; set; } = null!;
}

public class Cart
{
    public int    Id            { get; set; }
    public int    TotalQuantity { get; set; }
    public decimal Total        { get; set; }
    public List<CartProduct> Products { get; set; } = null!;
}

public class CartProduct
{
    public int    Id       { get; set; }
    public string Title    { get; set; } = null!;
    public int    Quantity { get; set; }
}
