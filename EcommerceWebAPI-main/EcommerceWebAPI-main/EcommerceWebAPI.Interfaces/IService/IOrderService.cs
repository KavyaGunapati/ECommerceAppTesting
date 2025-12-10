using EcommerceWebAPI.Models.DTOs;

namespace EcommerceWebAPI.Interfaces.IService
{
    public interface IOrderService
    {
        Task<Result<OrderResponse>> PlaceOrderAsync(string userId, CreateOrder dto);
        Task<Result<List<OrderResponse>>> GetOrdersByUserAsync(string userId);
        Task<Result<OrderResponse>> GetOrderByIdAsync(int id);
        Task<Result> CancelOrderAsync(int id);
    }
}
