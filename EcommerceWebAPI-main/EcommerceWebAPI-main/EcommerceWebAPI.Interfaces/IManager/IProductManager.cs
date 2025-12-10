using EcommerceWebAPI.Models.DTOs;

namespace EcommerceWebAPI.Interfaces.IManager
{
    public interface IProductManager
    {
        Task<Result<ProductResponse>> AddProductAsync(ProductResponse dto);
        Task<Result<List<ProductResponse>>> GetAllProductsAsync();
        Task<Result<ProductResponse>> GetProductByIdAsync(int id);
        Task<Result> UpdateProductAsync(int id, ProductResponse dto);
        Task<Result> DeleteProductAsync(int id);
    }

}
