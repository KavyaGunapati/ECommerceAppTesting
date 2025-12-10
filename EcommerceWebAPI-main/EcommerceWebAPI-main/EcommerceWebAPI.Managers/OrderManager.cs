
using AutoMapper;
using EcommerceWebAPI.Interfaces.IManager;
using EcommerceWebAPI.Interfaces.IRepository;
using EcommerceWebAPI.Models.Constants;
using EcommerceWebAPI.Models.DTOs;
using ECommerceWebAPI.DataAccess.Entities;

namespace EcommerceWebAPI.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public OrderManager(IGenericRepository<Order> orderRepository,
                            IGenericRepository<OrderItem> orderItemRepository,
                            IGenericRepository<Product> productRepository,
                            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Result<OrderResponse>> PlaceOrderAsync(string userId, CreateOrder dto)
        {
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 0
                };

                await _orderRepository.AddAsync(order);

                foreach (var item in dto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null) continue;

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity
                    };

                    order.TotalAmount += product.Price * item.Quantity;
                    await _orderItemRepository.AddAsync(orderItem);
                }

                var response = _mapper.Map<OrderResponse>(order);
                return new Result<OrderResponse> { Success = true, Message = "Order placed successfully", Data = response };
            }
            catch
            {
                return new Result<OrderResponse> { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }

        public async Task<Result<List<OrderResponse>>> GetOrdersByUserAsync(string userId)
        {
            try
            {
                var orders = await _orderRepository.FindAsync(o => o.UserId == userId);
                var response = _mapper.Map<List<OrderResponse>>(orders);
                return new Result<List<OrderResponse>> { Success = true, Message = $"{userId} Orders", Data = response };
            }
            catch
            {
                return new Result<List<OrderResponse>> { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }

        public async Task<Result<OrderResponse>> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return new Result<OrderResponse> { Success = false, Message = ErrorConstants.OrderNotFound };

                var response = _mapper.Map<OrderResponse>(order);
                return new Result<OrderResponse> { Success = true, Message = "Orders", Data = response };
            }
            catch
            {
                return new Result<OrderResponse> { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }

        public async Task<Result> CancelOrderAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return new Result { Success = false, Message = ErrorConstants.OrderNotFound };

                await _orderRepository.DeleteAsync(order);
                return new Result { Success = true, Message = "Order cancelled successfully" };
            }
            catch
            {
                return new Result { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }
    }
}
