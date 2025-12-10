using EcommerceWebAPI.Models.DTOs;

namespace EcommerceWebAPI.Interfaces.IManager
{ 
    public interface IOrderManager
    {
        Task<Result<OrderResponse>> PlaceOrderAsync(string userId, CreateOrder dto);
        Task<Result<List<OrderResponse>>> GetOrdersByUserAsync(string userId);
        Task<Result<OrderResponse>> GetOrderByIdAsync(int id);
        Task<Result> CancelOrderAsync(int id);
    }

}
