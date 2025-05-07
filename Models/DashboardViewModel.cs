using System.Collections.Generic;
using ADAProject.Models;

namespace ADAProject.Models
{
    public class DashboardViewModel
    {
        public List<Product>  ?Products  { get; set; }
        public List<PurchaseOrder> ?Orders { get; set; }
    }
}
