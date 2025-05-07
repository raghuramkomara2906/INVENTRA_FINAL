namespace ADAProject.Models
{
    // top-level JSON from https://dummyjson.com/products
    public class DummyJsonResponse
    {
        public List<DummyJsonProduct> Products { get; set; } = new();
    }

    // each product object
    public class DummyJsonProduct
    {
        public int    Id        { get; set; }
        public string Title     { get; set; } = "";
        public string Category  { get; set; } = "";
        public int    Stock     { get; set; }
        public string Thumbnail { get; set; } = "";
    }
}
