using AutoMapper;
using EcommerceWebAPI.Interfaces.IRepository;
using EcommerceWebAPI.Managers;
using EcommerceWebAPI.Models.Constants;
using EcommerceWebAPI.Models.DTOs;
using ECommerceWebAPI.DataAccess.Entities;
using FluentAssertions;
using Moq;

namespace ManagerTests
{
    [TestFixture]
    public class ProductManagerTests
    {
        private Mock<IGenericRepository<Product>> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private ProductManager _productManager;
        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IGenericRepository<Product>>();
            _mockMapper = new Mock<IMapper>();
            _productManager = new ProductManager(_mockRepository.Object, _mockMapper.Object);
        }
        [Test]
        public async Task AddProductAsync_Should_Add_And_Return_Mapped_Response()
        {
            //Arrange
            var productResponseDto = new ProductResponse { Id = 1, Name = "Test Product", Price = 100M, StockQuantity = 10, Description = "testing product" };
            var productEntity = new Product { Id = 1, Name = "Test Product", Price = 100M, StockQuantity = 10, Description = "testing product" };
            _mockMapper.Setup(m => m.Map<Product>(It.IsAny<ProductResponse>())).Returns(productEntity);
            _mockRepository.Setup(r => r.AddAsync(productEntity)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<ProductResponse>(productEntity)).Returns(productResponseDto);
            //Act
            var result = await _productManager.AddProductAsync(productResponseDto);
            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(productResponseDto);
            result.Message.Should().Be("Product added successfully");
            _mockRepository.Verify(r => r.AddAsync(productEntity), Times.Once);
        }
        [Test]
        public async Task AddProductAsync_When_Exception_Occurs_Should_Return_InternalServerError()
        {
            //Arrange
            var productResponseDto = new ProductResponse { Id = 1, Name = "Test Product", Price = 100M, StockQuantity = 10, Description = "testing product" };
            _mockMapper.Setup(m => m.Map<Product>(productResponseDto)).Throws(new Exception("Map Failed"));
            //Act
            var result = await _productManager.AddProductAsync(productResponseDto);
            //Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(ErrorConstants.InternalServerError);
        }
        [Test]
        public async Task GetAllProductsAsync_Should_Return_All_Products()
        {
            //Arrange
            var productEntities = new List<Product> {
                new Product { Id=1,Name="Test Product",Price=100M,StockQuantity=10,Description="testing product"} ,
                new Product { Id=2,Name="Another Product",Price=200M,StockQuantity=5,Description="another testing product"}
            };
            var productResponseDtos = new List<ProductResponse> {
                new ProductResponse { Id=1,Name="Test Product",Price=100M,StockQuantity=10,Description="testing product"} ,
                new ProductResponse { Id=2,Name="Another Product",Price=200M,StockQuantity=5,Description="another testing product"}
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(productEntities);
            _mockMapper.Setup(m => m.Map<List<ProductResponse>>(productEntities)).Returns(productResponseDtos);
            //Act
            var result = await _productManager.GetAllProductsAsync();
            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(productResponseDtos);
            result.Message.Should().Be("All Products");
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<List<ProductResponse>>(productEntities), Times.Once);


        }
        [Test]
        public async Task GetAllProductsAsync_Return_Null_When_No_Products()
        {
            //Arrange
            List<Product> productEntities = null;
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(productEntities);
            // AutoMapper will be called with null; typically returns null
            _mockMapper.Setup(m => m.Map<List<ProductResponse>>(productEntities)).Returns((List<ProductResponse>)null);
            //Act
            var result = await _productManager.GetAllProductsAsync();
            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            // Mapper IS called in current code, even with null
            _mockMapper.Verify(m => m.Map<List<ProductResponse>>(productEntities), Times.Once);
            result.Data.Should().BeNull();
        }
        [Test]
        public async Task GetAllProductsAsync_When_Exception_Occurs_Should_Return_InternalServerError()
        {
            //Arrange
            _mockRepository.Setup(r => r.GetAllAsync()).Throws(new Exception("DB Error"));
            //Act
            var result = await _productManager.GetAllProductsAsync();
            //Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(ErrorConstants.InternalServerError);


        }
        [Test]
        public async Task GetProductByIdAsync_When_product_Found()
        {
            //Arrange
            int productId = 1;
            var productEntity = new Product { Id = productId, Name = "Test Product", Price = 100M, StockQuantity = 10, Description = "testing product" };
            var productResponseDto = new ProductResponse { Id = productId, Name = "Test Product", Price = 100M, StockQuantity = 10, Description = "testing product" };
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(productEntity);
            _mockMapper.Setup(m => m.Map<ProductResponse>(productEntity)).Returns(productResponseDto);
            //Act
            var result = await _productManager.GetProductByIdAsync(productId);
            //Assert
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(productResponseDto);
            _mockMapper.Verify(m => m.Map<ProductResponse>(productEntity), Times.Once);
        }
        [Test]
        public async Task GetProductByIdAsync_When_product_Not_Found()
        {
            //Arrange
            int productId = 1;
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product)null);
            _mockMapper.Setup(m => m.Map<ProductResponse>(null)).Returns((ProductResponse)null);
            //Act
            var result = await _productManager.GetProductByIdAsync(productId);
            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            _mockMapper.Verify(m => m.Map<ProductResponse>(null), Times.Never);
        }
        [Test]
        public async Task GetProductByIdAsync_When_Exception_Occurs_Should_Return_InternalServerError()
        {
            //Arrange
            int productId = 1;
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).Throws(new Exception("Db Error"));
            //Act
            var result = await _productManager.GetProductByIdAsync(productId);
            //Assert
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be(ErrorConstants.InternalServerError);
        }
        [Test]
        public async Task GetProductByIdAsync_When_Invalid_Id_Should_Return_ProductNotFound()
        {
            //Arrange
            int productId = -1;
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product)null);
            //Act
            var result = await _productManager.GetProductByIdAsync(productId);
            //Assert
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be(ErrorConstants.ProductNotFound);
        }
        [Test]
        public async Task UpdateProduct_Returns_Updated_ProductResponse()
        {
            //Arrange
            var productId = 1;
            var existingProduct = new Product { Id = productId, Name = "Old Product", Price = 50M, StockQuantity = 5, Description = "old description" };
            var updatedProductResponse = new ProductResponse { Id = productId, Name = "Updated Product", Price = 150M, StockQuantity = 15, Description = "updated description" };
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
            _mockMapper.Setup(m => m.Map(updatedProductResponse, existingProduct)).Callback<ProductResponse, Product>((dest, src) =>
            {
                dest.Name = src.Name;
                dest.Price = src.Price;
                dest.StockQuantity = src.StockQuantity;
                dest.Description = src.Description;
            });
            _mockRepository.Setup(r => r.UpdateAsync(existingProduct)).Returns(Task.CompletedTask);
            //Act
            var result = await _productManager.UpdateProductAsync(productId, updatedProductResponse);
            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockMapper.Verify(m => m.Map(updatedProductResponse, existingProduct), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(existingProduct), Times.Once);
            existingProduct.Name.Should().Be(updatedProductResponse.Name);
            existingProduct.Price.Should().Be(updatedProductResponse.Price);
            existingProduct.StockQuantity.Should().Be(updatedProductResponse.StockQuantity);
            existingProduct.Description.Should().Be(updatedProductResponse.Description);
        }
        [Test]
        public async Task UpdateProduct_When_Product_Not_Found_Should_Return_ProductNotFound()
        {
            //Arrange
            var productId = 1;
            var updatedProductResponse = new ProductResponse { Id = productId, Name = "Updated Product", Price = 150M, StockQuantity = 15, Description = "updated description" };
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product)null);
            //Act
            var result = await _productManager.UpdateProductAsync(productId, updatedProductResponse);
            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be(ErrorConstants.ProductNotFound);
            _mockMapper.Verify(m => m.Map(It.IsAny<ProductResponse>(), It.IsAny<Product>()), Times.Never);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }
        [Test]
        public async Task UpdateProduct_When_Exception_Occurs_Should_Return_InternalServerError()
        {
            //Arrange
            var productId = 1;
            var existingProduct = new Product { Id = productId, Name = "Old Product", Price = 50M, StockQuantity = 5, Description = "old description" };
            var updatedProductResponse = new ProductResponse { Id = productId, Name = "Updated Product", Price = 150M, StockQuantity = 15, Description = "updated description" };
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
            _mockMapper.Setup(m => m.Map(updatedProductResponse, existingProduct)).Throws(new Exception("Mapping Error"));
            //Act
            var result = await _productManager.UpdateProductAsync(productId, updatedProductResponse);
            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be(ErrorConstants.InternalServerError);

        }
        [Test]
        public async Task DeleteProduct_When_Product_Exists_Should_Return_Success()
        {
            //Arrange
            var productId = 1;
            var existingProduct = new Product { Id = productId, Name = "Test Product", Price = 100M, StockQuantity = 10, Description = "testing product" };
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
            _mockRepository.Setup(r => r.DeleteAsync(existingProduct)).Returns(Task.CompletedTask);
            //Act
            var result = await _productManager.DeleteProductAsync(productId);
            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Product deleted successfully");
            _mockRepository.Verify(r => r.DeleteAsync(existingProduct), Times.Once);
            _mockRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);

        }
        [Test]
        public async Task DeleteProduct_When_Product_Not_Found_Should_Return_ProductNotFound()
        {
            //Arrange
            var productId = 1;
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product)null);
            //Act
            var result = await _productManager.DeleteProductAsync(productId);
            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be(ErrorConstants.ProductNotFound);
            _mockRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);

        }
    }
}


