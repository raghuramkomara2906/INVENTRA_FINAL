namespace ADAProject.Models
{
    // matches the JSON shape from https://northwind.vercel.app/api/suppliers
    public class SupplierApiModel
    {
        public int    id          { get; set; }
        public string companyName { get; set; } = "";
        public string contactName { get; set; } = "";
        public string phone       { get; set; } = "";
        // note: no email in that API, so we leave Email blank in DB
    }
}
