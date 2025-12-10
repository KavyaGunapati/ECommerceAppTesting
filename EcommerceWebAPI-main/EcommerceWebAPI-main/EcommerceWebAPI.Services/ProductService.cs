using EcommerceWebAPI.Interfaces.IManager;
using EcommerceWebAPI.Interfaces.IService;
using EcommerceWebAPI.Models.DTOs;

namespace EcommerceWebAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductManager _productManager;
        public ProductService(IProductManager productManager) {
            _productManager = productManager;
        }
        public async Task<Result<ProductResponse>> AddProductAsync(ProductResponse dto)
        {
            return await _productManager.AddProductAsync(dto);
        }

        public async Task<Result> DeleteProductAsync(int id)
        {
                return await (_productManager.DeleteProductAsync(id));
        }

        public async Task<Result<List<ProductResponse>>> GetAllProductsAsync()
        {
            return await _productManager.GetAllProductsAsync();
        }

        public async Task<Result<ProductResponse>> GetProductByIdAsync(int id)
        {
            return await _productManager.GetProductByIdAsync(id);
        }

        public async Task<Result> UpdateProductAsync(int id, ProductResponse dto)
        {
            return await _productManager.UpdateProductAsync(id, dto);
        }
    }
}
