using System.ComponentModel.DataAnnotations;

namespace JwtInfrastructure.Models
{
    public class OrderItems
    {
        [Key]
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string? Name { get; set; } = "";
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
    }
}
