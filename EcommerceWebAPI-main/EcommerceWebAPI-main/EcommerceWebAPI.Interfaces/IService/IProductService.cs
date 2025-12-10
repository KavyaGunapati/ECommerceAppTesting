using EcommerceWebAPI.Models.DTOs;

namespace EcommerceWebAPI.Interfaces.IService
{
    public interface IProductService
    {
        Task<Result<ProductResponse>> AddProductAsync(ProductResponse dto);
        Task<Result<List<ProductResponse>>> GetAllProductsAsync();
        Task<Result<ProductResponse>> GetProductByIdAsync(int id);
        Task<Result> UpdateProductAsync(int id, ProductResponse dto);
        Task<Result> DeleteProductAsync(int id);
    }
}
