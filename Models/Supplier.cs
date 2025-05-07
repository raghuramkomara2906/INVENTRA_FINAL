using System.ComponentModel.DataAnnotations;

namespace ADAProject.Models
{
    public class Supplier
    {
        public int    Id      { get; set; }

        [Required]
        public string Name    { get; set; } = "";

        [Required]
        public string Contact { get; set; } = "";

        [EmailAddress]
        public string Email   { get; set; } = "";

        public string Phone   { get; set; } = "";
    }
}
