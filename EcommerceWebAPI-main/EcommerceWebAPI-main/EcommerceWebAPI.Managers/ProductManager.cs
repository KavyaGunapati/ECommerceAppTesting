
using AutoMapper;
using EcommerceWebAPI.Interfaces.IManager;
using EcommerceWebAPI.Interfaces.IRepository;
using EcommerceWebAPI.Models.Constants;
using EcommerceWebAPI.Models.DTOs;
using ECommerceWebAPI.DataAccess.Entities;

namespace EcommerceWebAPI.Managers
{
    public class ProductManager : IProductManager
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly IMapper _mapper;

        public ProductManager(IGenericRepository<Product> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<ProductResponse>> AddProductAsync(ProductResponse dto)
        {
            try
            {
                var product = _mapper.Map<Product>(dto);
                await _repository.AddAsync(product);
                var response = _mapper.Map<ProductResponse>(product);
                return new Result<ProductResponse> { Success = true, Message = "Product added successfully", Data = response };
            }
            catch (Exception)
            {
                return new Result<ProductResponse> { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }
        public async Task<Result<List<ProductResponse>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _repository.GetAllAsync();
                var response = _mapper.Map<List<ProductResponse>>(products);
                return new Result<List<ProductResponse>> { Success=true,Message="All Products",Data = response};
            }
            catch (Exception)
            {
                return new Result<List<ProductResponse>> { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }

        public async Task<Result<ProductResponse>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                    return new Result<ProductResponse> { Success = false, Message = ErrorConstants.ProductNotFound };

                var response = _mapper.Map<ProductResponse>(product);
                return new Result<ProductResponse> { Success = true, Message = $"Products of Id={id}", Data = response };
            }
            catch (Exception)
            {
                return new Result<ProductResponse> { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }

        public async Task<Result> UpdateProductAsync(int id, ProductResponse dto)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                    return new Result { Success = false, Message = ErrorConstants.ProductNotFound };

                _mapper.Map(dto, product);
                await _repository.UpdateAsync(product);
                return new Result { Success = true, Message = "Product updated successfully" };
            }
            catch (Exception)
            {
                return new Result { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }
        public async Task<Result> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                    return new Result { Success = false, Message = ErrorConstants.ProductNotFound };

                await _repository.DeleteAsync(product);
                return new Result { Success = true, Message = "Product deleted successfully" };
            }
            catch (Exception)
            {
                return new Result { Success = false, Message = ErrorConstants.InternalServerError };
            }
        }
    }
}
