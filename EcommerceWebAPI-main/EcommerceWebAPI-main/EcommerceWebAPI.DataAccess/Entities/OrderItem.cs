
namespace ECommerceWebAPI.DataAccess.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        // Foreign Key for Product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Foreign Key for Order
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
