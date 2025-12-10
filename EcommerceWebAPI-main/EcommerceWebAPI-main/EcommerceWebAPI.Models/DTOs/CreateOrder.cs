namespace EcommerceWebAPI.Models.DTOs
{

    public class CreateOrder
    {
        public List<OrderItemResponse> Items { get; set; } = new();
    }

}
