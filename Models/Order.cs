using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAProject.Models
{
    public class Order
    {
        [Key]
        public string   Id    { get; set; } = string.Empty;

        public string   Text  { get; set; } = string.Empty;
        public int      Items { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal  Cost  { get; set; }

        public string   Status    { get; set; } = "New";

        // ←— ADD THIS:
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
