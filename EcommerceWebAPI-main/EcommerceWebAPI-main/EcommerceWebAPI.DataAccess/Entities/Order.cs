
namespace ECommerceWebAPI.DataAccess.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Foreign Key for User
        public string UserId { get; set; }
        public User User { get; set; }

        // Relationship: One Order → Many OrderItems
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
