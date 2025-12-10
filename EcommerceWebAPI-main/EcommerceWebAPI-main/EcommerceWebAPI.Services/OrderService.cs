using EcommerceWebAPI.Interfaces.IManager;
using EcommerceWebAPI.Interfaces.IService;
using EcommerceWebAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderManager _orderManager;
        public OrderService(IOrderManager orderManager) 
        {
            _orderManager = orderManager;
        }
        public async Task<Result> CancelOrderAsync(int id)
        {
           return await _orderManager.CancelOrderAsync(id);
        }

        public async Task<Result<OrderResponse>> GetOrderByIdAsync(int id)
        {
            return await _orderManager.GetOrderByIdAsync(id);
        }

        public async Task<Result<List<OrderResponse>>> GetOrdersByUserAsync(string userId)
        {
            return await _orderManager.GetOrdersByUserAsync(userId);
        }

        public async Task<Result<OrderResponse>> PlaceOrderAsync(string userId, CreateOrder dto)
        {
            return await _orderManager.PlaceOrderAsync(userId, dto);
        }
    }
}
