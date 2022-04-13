using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityApp.Models
{
    public enum OrderStatus
    {
        Created,
        NotPaid,
        InWork,        
        Shipped,
        Delivered
    }
    public class Order
    {
        public int OrderID { get; set; }
        public string? UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int Discount { get; set; }
        [NotMapped]
        public decimal TotalCost { get; set; }
        public List<OrderLine> Lines { get; set; } = new List<OrderLine>();

    }

    public class OrderLine
    {
        public long Id { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }        
        [NotMapped]
        public decimal Cost { get; set; }
    }
}
